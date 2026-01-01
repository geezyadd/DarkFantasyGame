using System.Collections.Generic;

namespace Features.CustomCodeGeneratorModule.Scripts.Editor.Core {
    public interface ICodeGeneratorWithSubGenerators {
        public List<string> GetSubGeneratorNames();
        public void ExecuteSubGeneratorByNames(GeneratorContext generatorContext, List<string> subGeneratorNames);
    }
}