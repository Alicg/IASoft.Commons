using System;
using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Unity;
using Unity.Lifetime;

namespace SVA.Notifications
{
    public class NotificationsModule : IModule
    {
        private readonly IUnityContainer unityContainer;
        private readonly IRegionManager regionManager;

        public const string NotificationsRegionName = "NotificationsRegion";

        public NotificationsModule(IUnityContainer unityContainer, IRegionManager regionManager)
        {
            this.unityContainer = unityContainer;
            this.regionManager = regionManager;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            LoadResources();
            this.InitTypes();

            var notificationsSystem = this.unityContainer.Resolve<INotificationSystem>();
            this.regionManager.RegisterViewWithRegion(NotificationsRegionName, () => notificationsSystem);
        }
        
        private void InitTypes()
        {
            this.unityContainer.RegisterType<INotificationSystem, NotificationViewModel>(new ContainerControlledLifetimeManager());
        }

        private static void LoadResources()
        {
            var dict = new ResourceDictionary { Source = new Uri("/SVA.Notifications;component/NotificationResources.xaml", UriKind.RelativeOrAbsolute) };
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
