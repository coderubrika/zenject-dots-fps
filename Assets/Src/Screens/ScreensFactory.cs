using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace TestRPG
{
    public class ScreensFactory : IFactory<Type, BaseScreen>
    {
        private readonly Dictionary<Type, BaseScreen> screens = new();
        private readonly string screensResourcePath;
        private readonly DiContainer container;
        private readonly Transform screensRoot;
        
        public ScreensFactory(
            DiContainer container, 
            (string screensResourcePath, string screenRootName) args)
        {
            this.container = container;
            screensResourcePath = args.screensResourcePath;
            
            screensRoot = new GameObject(args.screenRootName).transform;
            Object.DontDestroyOnLoad(screensRoot.gameObject);
        }

        public BaseScreen Create(Type screenType)
        {
            if (screens.TryGetValue(screenType, out BaseScreen screen))
                return screen;
            
            BaseScreen prefab = Resources.Load<BaseScreen>($"{screensResourcePath}/{screenType.Name}");
            BaseScreen newScreen = (BaseScreen)container.InstantiatePrefabForComponent(
                screenType, prefab, screensRoot, Array.Empty<object>());
            newScreen.gameObject.SetActive(false);
            screens.Add(screenType, newScreen);
            
            return newScreen;
        }
    }
}