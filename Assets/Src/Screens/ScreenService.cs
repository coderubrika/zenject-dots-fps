using System;
using Zenject;

namespace TestRPG
{
    public class ScreenService
    {
        private readonly IFactory<Type, BaseScreen> screensFactory;
        private BaseScreen currentScreen;

        public ScreenService(IFactory<Type, BaseScreen> screensFactory)
        {
            this.screensFactory = screensFactory;
        }
        
        public void GoTo<TScreen>()
            where TScreen : BaseScreen
        {
            if (currentScreen != null && typeof(TScreen) == currentScreen.GetType())
                return;

            if (currentScreen != null)
                currentScreen.Hide();
            
            currentScreen = screensFactory.Create(typeof(TScreen));
            currentScreen.Show();
        }
    }
}