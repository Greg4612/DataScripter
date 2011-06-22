using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.UI.VSIntegration;

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

            IObjectExplorerService objectExplorer = ServiceCache.GetObjectExplorer();
            IObjectExplorerEventProvider provider = (IObjectExplorerEventProvider)objectExplorer.GetService(typeof(IObjectExplorerEventProvider));
            provider.SelectionChanged += new NodesChangedEventHandler(Provider_SelectionChanged);
        }

        private void Provider_SelectionChanged(object sender, NodesChangedEventArgs args)
        {
            INodeInformation[] nodes;
            int nodeCount;

            IObjectExplorerService objectExplorer = ServiceCache.GetObjectExplorer();
            objectExplorer.GetSelectedNodes(out nodeCount, out nodes);
            INodeInformation node = (nodeCount > 0 ? nodes[0] : null);

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