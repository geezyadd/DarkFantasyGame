using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IncrementalSourceGenerator.Utils;
using RSG.Muffin.CustomCodeGeneratorModule.Scripts.Editor.Core;
using UnityEditor;
using UnityEngine.InputSystem;
using ICodeGenerator = RSG.Muffin.CustomCodeGeneratorModule.Scripts.Editor.Core.ICodeGenerator;
using Tools = IncrementalSourceGenerator.Utils.Tools;

namespace Features.Input.Scripts.Editor {
    [Generator]
    public class InputRealizationGenerator : ICodeGenerator {
        private readonly string _namespace = GENERATED_FILES_PATH.Replace("Assets/", "").Replace("/", ".");
        private const string INPUT_ACTIONS_NAME = nameof(InputActions);
        private const string INPUT_SYSTEM = "InputService";
        private const string INPUT_SYSTEM_INTERFACE = "IInputService";
        //You need to also change path in InputActionAsset file
        private const string GENERATED_FILES_PATH = "Assets/Core/InputModule/Scripts/Generated";

        public void Execute(GeneratorContext context) {
            context.OverrideFolderPath(GENERATED_FILES_PATH);
            context.AddCode(Tools.BuildFileName(INPUT_SYSTEM_INTERFACE), Tools.GetSourceText(InterfaceInputSystemSource));
            context.AddCode(Tools.BuildFileName(INPUT_SYSTEM), Tools.GetSourceText(InputSystemSource));
        }

        private string InterfaceInputSystemSource(StringBuilder builder, StringWriter writer, IndentedTextWriter text) {
            InputActionAsset inputActionAsset = GetInputActionAsset(INPUT_ACTIONS_NAME);
            if (inputActionAsset == null)
                return string.Empty;

            List<string> mapNames = inputActionAsset.actionMaps.Select(m => m.name.Replace(" ", "")).ToList();
            List<InputAction> actions = new();
            foreach (InputActionMap actionMap in inputActionAsset.actionMaps)
                actions.AddRange(actionMap.actions);

            text.AddUsing("UnityEngine");
            text.AddUsing("RSG.Muffin.InputSubmodule.InputModule.Core.Scripts");
            text.AddUsing("UnityEngine.InputSystem");

            text.AddNamespace(_namespace);
            {
                text.OpenBlock();
                text.Write("public interface " + INPUT_SYSTEM_INTERFACE);
                {
                    text.OpenBlock();
                    foreach (InputAction action in actions)
                        AddPublicVariable(text, action.expectedControlType switch {
                            "Vector2" => "InputVector2Actions",
                            "Vector3" => "InputVector3Actions",
                            _ => "InputDefaultActions"
                        }, action.name.Replace(" ", ""), true);
                    
                    foreach (InputAction action in actions) {
                        if (action.expectedControlType == "Vector2")
                            AddVector2ReadValue(text, INPUT_ACTIONS_NAME, action.actionMap.name, action.name, true);

                        if (action.expectedControlType == "Vector3")
                            AddVector3ReadValue(text, INPUT_ACTIONS_NAME, action.actionMap.name, action.name, true);
                    }

                    AddEnable(text, INPUT_ACTIONS_NAME, true);
                    AddDisable(text, INPUT_ACTIONS_NAME, true);

                    foreach (string mapName in mapNames) {
                        AddEnableMap(text, INPUT_ACTIONS_NAME, mapName, true);
                        AddDisableMap(text, INPUT_ACTIONS_NAME, mapName, true);
                        AddIsEnabledMap(text, mapName, true);
                    }
                    
                    text.WriteLine("public bool IsPointerOverGameObject();");

                    text.CloseBlock();
                }

                text.CloseBlock();
            }

            return writer.ToString();
        }

        private string InputSystemSource(StringBuilder builder, StringWriter writer, IndentedTextWriter text) {
            InputActionAsset inputActionAsset = GetInputActionAsset(INPUT_ACTIONS_NAME);
            if (inputActionAsset == null)
                return string.Empty;

            List<string> mapNames = inputActionAsset.actionMaps.Select(m => m.name.Replace(" ", "")).ToList();
            List<InputAction> actions = new();
            foreach (InputActionMap actionMap in inputActionAsset.actionMaps)
                actions.AddRange(actionMap.actions);

            text.AddUsing("UnityEngine");
            text.AddUsing("UnityEngine.InputSystem");
            text.AddUsing("RSG.Muffin.InputSubmodule.InputModule.Core.Scripts");
            text.AddUsing("Zenject");
            text.AddUsing("System");
            text.AddUsing("UnityEngine.EventSystems");
            text.AddUsing("System.Linq");

            text.AddNamespace(_namespace);
            {
                text.OpenBlock();
                text.AddPublicClass(INPUT_SYSTEM);
                AddInterfaces(text, mapNames);
                {
                    text.OpenBlock();

                    AddPrivateVariable(text, INPUT_ACTIONS_NAME, INPUT_ACTIONS_NAME);
                    
                    foreach (InputAction action in actions)
                        AddPublicVariable(text, action.expectedControlType switch {
                            "Vector2" => "InputVector2Actions",
                            "Vector3" => "InputVector3Actions",
                            _ => "InputDefaultActions"
                        }, action.name.Replace(" ", ""));

                    AddConstructor(text, INPUT_SYSTEM, new List<string> {INPUT_ACTIONS_NAME});

                    AddInitialize(text);
                    AddDispose(text);
                    
                    foreach (InputAction action in actions) {
                        if (action.expectedControlType == "Vector2")
                            AddVector2ReadValue(text, INPUT_ACTIONS_NAME, action.actionMap.name, action.name);

                        if (action.expectedControlType == "Vector3")
                            AddVector3ReadValue(text, INPUT_ACTIONS_NAME, action.actionMap.name, action.name);
                    }

                    AddEnable(text, INPUT_ACTIONS_NAME);
                    AddDisable(text, INPUT_ACTIONS_NAME);

                    AddSetControlsCallback(text, INPUT_ACTIONS_NAME, mapNames);
                    AddRemoveControlsCallback(text, INPUT_ACTIONS_NAME, mapNames);

                    foreach (string mapName in mapNames) {
                        AddEnableMap(text, INPUT_ACTIONS_NAME, mapName);
                        AddDisableMap(text, INPUT_ACTIONS_NAME, mapName);
                        AddIsEnabledMap(text, mapName, false);
                    }

                    foreach (InputAction action in actions)
                        AddOnMethod(text, action);
                    
                    text.WriteLine("public bool IsPointerOverGameObject()");
                    {
                        text.OpenBlock();
                        text.WriteLine("return EventSystem.current.IsPointerOverGameObject();");
                        text.CloseBlock();
                    }
                    
                    text.CloseBlock();
                }

                text.CloseBlock();
            }

            return writer.ToString();
        }

        private static void AddIsEnabledMap(IndentedTextWriter text, string mapName, bool isInterface) {
            if (isInterface)
                text.WriteLine($"public bool IsEnabled{mapName}();");
            else {
                text.WriteLine($"public bool IsEnabled{mapName}()");
                {
                    text.OpenBlock();
                    string inputActionsNameVariable = INPUT_ACTIONS_NAME.Remove(0, 1).Insert(0, INPUT_ACTIONS_NAME[0].ToString().ToLower());
                    text.WriteLine($"return _{inputActionsNameVariable}.{mapName}.enabled;");
                    text.CloseBlock();
                }
            }
        }

        private void AddInitialize(IndentedTextWriter text) {
            text.WriteLine("public void Initialize() =>");
            text.WriteLine("    Enable();");
        }

        private void AddDispose(IndentedTextWriter text) {
            text.WriteLine("public void Dispose() =>");
            text.WriteLine("    Disable();");
        }

        private void AddPrivateVariable(IndentedTextWriter text, string variableType, string variableName) {
            string muffinInputActionsName = variableName.Remove(0, 1).Insert(0, variableName[0].ToString().ToLower());
            text.WriteLine($"private {variableType} _{muffinInputActionsName};");
        }

        private void AddPublicVariable(IndentedTextWriter text, string variableType, string variableName, bool isInterface = false) =>
            text.WriteLine(isInterface
                ? $"public {variableType} {variableName} {{ get; set; }}"
                : $"public {variableType} {variableName} {{ get; set; }} = new();");

        private void AddConstructor(IndentedTextWriter text, string className, List<string> variableClassNames) {
            text.Write($"public {className}");
            text.AddOpenRoundBrace();
            for (int index = 0; index < variableClassNames.Count; index++) {
                string variableClassName = variableClassNames[index];
                string muffinInputActionsName = variableClassName;
                muffinInputActionsName = muffinInputActionsName.Remove(0, 1).Insert(0, muffinInputActionsName[0].ToString().ToLower());
                text.Write($"{variableClassName} {muffinInputActionsName}");
                if(index != variableClassNames.Count -1)
                    text.Write(",");
            }

            text.AddCloseRoundBrace();
            text.OpenBlock();
            
            foreach (string variableClassName in variableClassNames) {
                string muffinInputActionsName = variableClassName;
                muffinInputActionsName = muffinInputActionsName.Remove(0, 1).Insert(0, muffinInputActionsName[0].ToString().ToLower());
                text.WriteLine($"_{muffinInputActionsName} = {muffinInputActionsName};");
            }
            
            text.CloseBlock();
        }

        private void AddInterfaces(IndentedTextWriter text, List<string> allMapNames) {
            string allInterfaces = string.Empty;
            foreach (string mapAction in allMapNames)
                allInterfaces += INPUT_ACTIONS_NAME + ".I" + mapAction + "Actions, ";

            allInterfaces += INPUT_SYSTEM_INTERFACE + ", IInitializable, IDisposable";
            text.AddInherited(allInterfaces);
        }

        private InputActionAsset GetInputActionAsset(string inputActionsName) {
            string[] findAssets = AssetDatabase.FindAssets(inputActionsName);
            return findAssets.Select(GetAssetByID).First(assetByID => assetByID != null);
        }

        private InputActionAsset GetAssetByID(string assetsGuid) {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetsGuid);
            InputActionAsset loadedAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(assetPath);
            return loadedAsset;
        }
        
        private void AddVector2ReadValue(IndentedTextWriter text, string inputActionsName, string mapName, string actionName, bool isInterface = false) {
            if (isInterface)
                text.WriteLine($"public Vector2 {actionName}Vector2ReadValue();");
            else {
                text.WriteLine($"public Vector2 {actionName}Vector2ReadValue()");
                text.OpenBlock();
                text.WriteLine($"return {actionName}.IsEnabled() ? _{inputActionsName.Remove(0, 1).Insert(0, inputActionsName[0].ToString().ToLower())}.{mapName}.{actionName}.ReadValue<Vector2>() : Vector2.zero;");
                text.CloseBlock();
            }
        }
        
        private void AddVector3ReadValue(IndentedTextWriter text, string inputActionsName, string mapName, string actionName, bool isInterface = false) {
            if (isInterface)
                text.WriteLine($"public Vector3 {actionName}Vector3ReadValue();");
            else {
                text.WriteLine($"public Vector3 {actionName}Vector3ReadValue()");
                text.OpenBlock();
                text.WriteLine($"return {actionName}.IsEnabled() ? _{inputActionsName.Remove(0, 1).Insert(0, inputActionsName[0].ToString().ToLower())}.{mapName}.{actionName}.ReadValue<Vector3>() : Vector3.zero;");
                text.CloseBlock();
            }
        }

        private void AddEnable(IndentedTextWriter text, string inputActionsName, bool isInterface = false) {
            if (isInterface)
                text.WriteLine("public void Enable();");
            else {
                text.WriteLine("public void Enable()");
                text.OpenBlock();
                text.WriteLine($"_{inputActionsName.Remove(0, 1).Insert(0, inputActionsName[0].ToString().ToLower())}.Enable();");
                text.WriteLine("SetControlsCallback();");
                text.CloseBlock();
            }
        }

        private void AddDisable(IndentedTextWriter text, string inputActionsName, bool isInterface = false) {
            if (isInterface)
                text.WriteLine("public void Disable();");
            else {
                text.WriteLine("public void Disable()");
                text.OpenBlock();
                text.WriteLine($"_{inputActionsName.Remove(0, 1).Insert(0, inputActionsName[0].ToString().ToLower())}.Disable();");
                text.WriteLine("RemoveControlsCallback();");
                text.CloseBlock();
            }
        }

        private void AddSetControlsCallback(IndentedTextWriter text, string muffinInputActionsName, List<string> allMapNames) {
            text.WriteLine("private void SetControlsCallback()");
            text.OpenBlock();
            foreach (string mapName in allMapNames)
                text.WriteLine(
                    $"_{muffinInputActionsName.Remove(0, 1).Insert(0, muffinInputActionsName[0].ToString().ToLower())}.{mapName}.SetCallbacks(this);");

            text.CloseBlock();
        }

        private void AddRemoveControlsCallback(IndentedTextWriter text, string muffinInputActionsName, List<string> allMapNames) {
            text.WriteLine("private void RemoveControlsCallback()");
            text.OpenBlock();
            foreach (string mapName in allMapNames)
                text.WriteLine(
                    $"_{muffinInputActionsName.Remove(0, 1).Insert(0, muffinInputActionsName[0].ToString().ToLower())}.{mapName}.RemoveCallbacks(this);");

            text.CloseBlock();
        }

        private void AddEnableMap(IndentedTextWriter text, string inputActionsName, string mapName, bool isInterface = false) {
            if (isInterface)
                text.WriteLine($"public void Enable{mapName}();");
            else {
                text.WriteLine($"public void Enable{mapName}()");
                text.OpenBlock();
                string inputActionsNameVariable = inputActionsName.Remove(0, 1).Insert(0, inputActionsName[0].ToString().ToLower());
                text.WriteLine($"_{inputActionsNameVariable}.{mapName}.Enable();");
                text.WriteLine($"_{inputActionsNameVariable}.{mapName}.SetCallbacks(this);");
                text.CloseBlock();
            }
        }

        private void AddDisableMap(IndentedTextWriter text, string inputActionsName, string mapName, bool isInterface = false) {
            if (isInterface)
                text.WriteLine($"public void Disable{mapName}();");
            else {
                text.WriteLine($"public void Disable{mapName}()");
                text.OpenBlock();
                string inputActionsNameVariable = inputActionsName.Remove(0, 1).Insert(0, inputActionsName[0].ToString().ToLower());
                text.WriteLine($"_{inputActionsNameVariable}.{mapName}.Disable();");
                text.WriteLine($"_{inputActionsNameVariable}.{mapName}.RemoveCallbacks(this);");
                text.CloseBlock();
            }
        }

        private void AddOnMethod(IndentedTextWriter text, InputAction action) {
            string actionName = action.name.Replace(" ", "");
            text.WriteLine("public void On" + actionName + "(InputAction.CallbackContext context)");
            text.OpenBlock();
            
            text.AddIfWithBrace();
            text.Write(actionName + ".IsEnabled()");
            text.AddCloseRoundBrace();
            text.OpenBlock();

            text.AddIfWithBrace();
            text.Write("context.started");
            text.AddCloseRoundBrace();
            text.WriteLine(actionName + ".Started?.Invoke();");

            text.AddIfWithBrace();
            text.Write("context.performed");
            text.AddCloseRoundBrace();
            text.WriteLine(actionName + ".Performed?.Invoke();");

            text.AddIfWithBrace();
            text.Write("context.canceled");
            text.AddCloseRoundBrace();
            text.WriteLine(actionName + ".Canceled?.Invoke();");

            if (action.expectedControlType == "Vector2") {
                text.AddIfWithBrace();
                text.Write("context.started");
                text.AddCloseRoundBrace();
                text.WriteLine(actionName + ".VectorChangedStarted?.Invoke(context.ReadValue<Vector2>());");

                text.AddIfWithBrace();
                text.Write("context.performed");
                text.AddCloseRoundBrace();
                text.WriteLine(actionName + ".VectorChangedPerformed?.Invoke(context.ReadValue<Vector2>());");

                text.AddIfWithBrace();
                text.Write("context.canceled");
                text.AddCloseRoundBrace();
                text.WriteLine(actionName + ".VectorChangedCanceled?.Invoke(context.ReadValue<Vector2>());");
            }
            else if (action.expectedControlType == "Vector3") {
                text.AddIfWithBrace();
                text.Write("context.started");
                text.AddCloseRoundBrace();
                text.WriteLine(actionName + ".VectorChangedStarted?.Invoke(context.ReadValue<Vector3>());");

                text.AddIfWithBrace();
                text.Write("context.performed");
                text.AddCloseRoundBrace();
                text.WriteLine(actionName + ".VectorChangedPerformed?.Invoke(context.ReadValue<Vector3>());");

                text.AddIfWithBrace();
                text.Write("context.canceled");
                text.AddCloseRoundBrace();
                text.WriteLine(actionName + ".VectorChangedCanceled?.Invoke(context.ReadValue<Vector3>());");
            }

            text.CloseBlock();
            text.CloseBlock();
        }
    }
}