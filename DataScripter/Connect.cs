using System;
using System.Text.RegularExpressions;
using EnvDTE;
using Extensibility;
using Microsoft.SqlServer.Management.SqlStudio.Explorer;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;

namespace iucon.ssms.DataScripter
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2
    {
        private AddIn _addInInstance;
        private HierarchyObject _tableMenu = null;
        private Regex _tableRegex = new Regex(@"^Server\[[^\]]*\]/Database\[[^\]]*\]/Table\[[^\]]*\]$");

        public Connect()
        {
        }

        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            _addInInstance = (AddIn)addInInst;

            // search ServiceCache.ServiceProvider.GetService ->
            // http://sqlblog.com/blogs/jonathan_kehayias/archive/2009/08/22/sql-2008-r2-breaks-ssms-addins.aspx

            ObjectExplorerService objectExplorer = (ObjectExplorerService)ServiceCache.ServiceProvider.GetService(typeof(IObjectExplorerService));

            // for some reason calling GetSelectedNodes forces to add ContextService to ObjectExplorerService.Container.Components
            int count = objectExplorer.Container.Components.Count;
            int nodeCount; INodeInformation[] nodes;
            objectExplorer.GetSelectedNodes(out nodeCount, out nodes);
            count = nodeCount; count = nodes.Length;
            count = objectExplorer.Container.Components.Count;

            ContextService contextService = (ContextService)objectExplorer.Container.Components[1];
            INavigationContextProvider provider = contextService.ObjectExplorerContext;

            provider.CurrentContextChanged += new NodesChangedEventHandler(Provider_SelectionChanged);
        }

        private void Provider_SelectionChanged(object sender, NodesChangedEventArgs args)
        {
            INavigationContextProvider provider = (INavigationContextProvider)sender;
            INodeInformation node = (args.ChangedNodes.Count > 0 ? args.ChangedNodes[0] : null);

            if (_tableMenu == null &&
                _tableRegex.IsMatch(node.Context))
            {
                _tableMenu = (HierarchyObject)node.GetService(typeof(IMenuHandler));

                MenuItem item = new MenuItem();
                _tableMenu.AddChild(string.Empty, item);
            }
        }

        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
        }

        public void OnAddInsUpdate(ref Array custom)
        {
        }

        public void OnStartupComplete(ref Array custom)
        {
        }

        public void OnBeginShutdown(ref Array custom)
        {
        }
    }
}