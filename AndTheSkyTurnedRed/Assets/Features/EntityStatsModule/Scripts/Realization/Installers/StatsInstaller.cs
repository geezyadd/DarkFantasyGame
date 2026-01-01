using Features.EntityStatsModule.Scripts.Realization.Factories;
using Features.EntityStatsModule.Scripts.StatsEntity.Factories;
using Zenject;

namespace Features.EntityStatsModule.Scripts.Realization.Installers {
    public class StatsInstaller : Installer<StatsInstaller> {
        public override void InstallBindings() {
            Container.Bind<IStatEntityFactory<EntityStatType>>()
                .To<EntityStatEntityFactory>()
                .AsSingle();
            
            Container.Bind<IStatFactory<EntityStatType>>()
                .To<EntityStatFactory>()
                .AsSingle();
        }
    }
}
