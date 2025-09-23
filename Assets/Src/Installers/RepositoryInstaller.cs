using UnityEngine;
using Zenject;

namespace TestRPG.Installers
{
    [CreateAssetMenu(fileName = "RepositoryInstaller", menuName = "Installers/RepositoryInstaller")]
    public class RepositoryInstaller : ScriptableObjectInstaller<RepositoryInstaller>
    {
        [SerializeField] private StartGameSettingsRepository startGameSettingsRepository;

        public override void InstallBindings()
        {
            Container.BindInstance(startGameSettingsRepository);
        }
    }
}