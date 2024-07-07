using Service_27;
using Service_31;
using Service_28;
using Service_85;
using _2EService;
using Service22;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using Service34;
using Service36;
using Service37;
using System.IO;
using sTemplateCreater;
using Microsoft.Win32;
using System.Xml;
using System.Text;
using System.Threading;
using static Service_31.Service31;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading.Tasks;


namespace sTemplateCreater
{
   
    public class TreeNode
    {
        public string Header { get; set; }
        public string GroupBoxName { get; set; }  // Add this if possible
        public ObservableCollection<TreeNode> Children { get; set; }

        // Add a property to indicate whether the node is highlighted
        public bool IsHighlighted { get; set; }


        public TreeNode()
        {
            Children = new ObservableCollection<TreeNode>();
           
            
        }

        public TreeNode(String header)
        {
            Header = header;
            Children = new ObservableCollection<TreeNode>();

        }
    }
   
    public static class VisualTreeHelperExtensions
    {
        public static T GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            Type type = typeof(T);
            if (element == null)
            {
                return null;
            }
            DependencyObject parent = VisualTreeHelper.GetParent(element);

            while (parent != null)
            {
                if (parent.GetType() == type)
                {
                    return parent as T;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }


    public partial class MainWindow : Window
    {
        private XDocument _currentXml;
        private Dictionary<string, string> data;
        private TreeViewItem _selectedTreeViewItem;
        private DispatcherTimer longPressTimer;
        // Declare DynamicTextBoxesPanel2 as a field
        private StackPanel DynamicTextBoxesPanel2;
        private StackPanel DynamicTextBoxesPanel3;
        private StackPanel DynamicTextBoxesPanel4;
        private string lastSelectedServiceType = "";
        private string lastSavedFilePath;


        #region LabelDeclarations
        public string GroupBoxHeader { get; set; }
        public string TemplateName { get; set; }
        public string OEM { get; set; }
        public string CommunicationProtocol { get; set; }
        public string BaudRate { get; set; }
        public string Types { get; set; }
        public string CommunicationDevice { get; set; }
        public string PhysicalCANID { get; set; }
        public string ResponseCANID { get; set; }
        public string FunctionalCANID { get; set; }

        public string TypeName { get; set; }

        public ObservableCollection<TreeNode> TreeNodes { get; set; }
        #endregion


        public MainWindow()
        {
            InitializeComponent();
            treeView.DataContext = this; // Assuming 'this' is your ViewModel or code-behind
            TypesComboBox.DataContext = this;
            TreeNodes = new ObservableCollection<TreeNode>();
            treeView.ItemsSource = TreeNodes;
            this.Loaded += MainWindow_Loaded;

            lastSelectedServiceType = Properties.Settings.Default.AllSelectedServiceType;

          
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            treeView.MouseRightButtonDown += TreeView_MouseRightButtonDown;
            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
        }
        #region ButtonClick
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TypesComboBox.SelectedItem != null)
            {
                TypesComboBox.Items.Remove(TypesComboBox.SelectedItem);
            }
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            TypesComboBox.Items.Add(TypesTextBox.Text);
        }
        private void OEMTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {


        }
        private void Install_Button_Click(Object sender, RoutedEventArgs e)
        {
            if (_currentXml == null)
            {
                MessageBox.Show("There is no XML to export. Please generate the data first.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string programDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string directory = System.IO.Path.Combine(programDataDirectory, "TempleteCreator");
            string fileName = "AutoSavedServices.sTemp";  // Name of the file
            string fullPath = System.IO.Path.Combine(directory, fileName);

            try
            {
                // Ensure the directory exists
                System.IO.Directory.CreateDirectory(directory);

                // Save the XML file directly to the specified path
                _currentXml.Save(fullPath);

                MessageBox.Show($"XML successfully installed to {fullPath}", "Install Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access denied: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        //private void LoadXmlToTreeView(string xmlContent, TreeView treeView)
        //{
        //    try
        //    {
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.LoadXml(xmlContent);

        //        // Prepare a new list to hold the items
        //        List<TreeViewItem> items = new List<TreeViewItem>();

        //        // Create a TreeViewItem for the root element
        //        TreeViewItem rootNode = new TreeViewItem();
        //        rootNode.Header = xmlDoc.DocumentElement.Name;

        //        // Add child nodes
        //        AddXmlNode(xmlDoc.DocumentElement, rootNode);

        //        // Add the root node to the list
        //        items.Add(rootNode);

        //        // Set the ItemsSource to the new list
        //        treeView.ItemsSource = items;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Failed to parse XML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}


        //private void AddXmlNode(XmlNode xmlNode, TreeViewItem parentNode, TreeNode parentTreeNode = null)
        //{
        //    foreach (XmlNode node in xmlNode.ChildNodes)
        //    {
        //        // Combine node name with attributes to form the header
        //        string header = node.Name;
        //        if (node.Attributes != null && node.Attributes.Count > 0)
        //        {
        //            List<string> attributes = new List<string>();
        //            foreach (XmlAttribute attr in node.Attributes)
        //            {
        //                attributes.Add($"{attr.Name}: {attr.Value}");
        //            }
        //            header += " [" + string.Join(", ", attributes) + "]";
        //        }

        //        // Append text content directly to the header if it's the only child and is text
        //        if (!string.IsNullOrWhiteSpace(node.InnerText) && node.ChildNodes.Count == 1 && node.FirstChild.NodeType == XmlNodeType.Text)
        //        {
        //            header += $" : {node.InnerText}";
        //        }

        //        TreeViewItem childNode = new TreeViewItem { Header = header };
        //        TreeNode treeNode = new TreeNode(header); // Initialize your TreeNode with the header

        //        if (parentTreeNode != null)
        //        {
        //            parentTreeNode.Children.Add(treeNode); // Maintain the hierarchical structure
        //        }

        //        childNode.DataContext = treeNode; // Set the DataContext to your TreeNode

        //        // Recursively add child nodes, passing the newly created TreeNode as the parent
        //        // Pass the TreeNode to maintain context
        //        if (!(node.ChildNodes.Count == 1 && node.FirstChild.NodeType == XmlNodeType.Text))
        //        {
        //            AddXmlNode(node, childNode, treeNode);
        //        }
        //        parentNode.Items.Add(childNode);
        //    }
        //}

        private void LoadXmlToTreeView(string xmlContent1, TreeView treeView)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent1);

                TreeNode rootNode = new TreeNode(xmlDoc.DocumentElement.Name);
                AddXmlNode(xmlDoc.DocumentElement, rootNode);

                // Set the ItemsSource to just the root node in a list (your TreeView should bind to TreeNode.Children)
                treeView.ItemsSource = new List<TreeNode> { rootNode };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse XML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddXmlNode(XmlNode xmlNode, TreeNode parentNode)
        {
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                string header = node.Name;
                List<string> attributes = new List<string>();
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        attributes.Add($"{attr.Name}: {attr.Value}");
                    }
                    header += string.Join(", ", attributes);
                }

                // Append text content directly to the header if it's the only child and is text
                if (node.ChildNodes.Count == 1 && node.FirstChild.NodeType == XmlNodeType.Text)
                {
                    header += $" : {node.InnerText}";
                }

                TreeNode childNode = new TreeNode(header);
                parentNode.Children.Add(childNode);

                // Recursively add child nodes
                if (!(node.ChildNodes.Count == 1 && node.FirstChild.NodeType == XmlNodeType.Text))
                {
                    AddXmlNode(node, childNode);
                }
            }
        }



        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {

        }
        private void TextBox_TextChanged_4(object sender, TextChangedEventArgs e)
        {

        }
        private void Export_Button_Click(object sender, RoutedEventArgs e)
        {
            RunCommand();
        }


        private void RunCommand()
        {
            // Prompt user to select the output directory
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string outputDirectory = dialog.SelectedPath.TrimEnd('\\'); // Ensure no trailing backslash
                    string batchFilePath = @"F:\SlokiTemplets\CopingFiles\installer.bat";

                    if (!File.Exists(batchFilePath))
                    {
                        MessageBox.Show("Batch file not found: " + batchFilePath, "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Prepare the command line arguments
                    string arguments = $"/c \"\"{batchFilePath}\" \"{outputDirectory}\"\"";

                    ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
                    {
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = Path.GetDirectoryName(batchFilePath) // Set the working directory to the batch file's directory
                    };

                    using (Process process = new Process { StartInfo = processStartInfo })
                    {
                        var output = new StringBuilder();
                        var error = new StringBuilder();

                        process.OutputDataReceived += (sender, args) =>
                        {
                            if (args.Data != null)
                            {
                                output.AppendLine(args.Data);
                            }
                        };

                        process.ErrorDataReceived += (sender, args) =>
                        {
                            if (args.Data != null)
                            {
                                error.AppendLine(args.Data);
                            }
                        };

                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                       




                    }

                    string expectedExePath = Path.Combine(outputDirectory, "SlokiFlasherTemplet.exe");
                    bool fileExists = File.Exists(expectedExePath);

                    if (fileExists)
                    {
                        MessageBox.Show($"Executable created successfully at {expectedExePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to create executable. Check log files and output for errors.", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    MessageBox.Show("Process completed. Check the results in the output directory", "Process Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No output directory selected. Operation cancelled.", "Operation Cancelled", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }














        private string GetUserDefinedOutputDirectory()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "Select the folder where you want to save the output file.";

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    return dialog.FileName;
                }
                else
                {
                    return null; // User canceled or didn't select a folder, handle appropriately.
                }
            }
        }


        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            GenerateTreeView();
            _currentXml = GenerateXml();

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "sTemp Files (*.sTemp)|*.sTemp",
                DefaultExt = "sTemp",
                Title = "Save XML File",
                InitialDirectory = @"F:\",
                FileName = "AutoSavedServices.sTemp"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                 _currentXml.Save(saveFileDialog.FileName);
                 Properties.Settings.Default.LastSavedFilePath = saveFileDialog.FileName;
                 Properties.Settings.Default.Save();
                 MessageBox.Show($"XML successfully saved to {saveFileDialog.FileName}", "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            DisplayXml(_currentXml);
        }


        #endregion

        #region TreeviewGenerate
        private void GenerateTreeView()
        {
            if (TreeNodes.Count == 0)
            {
                TreeNodes.Add(new TreeNode { Header = "TemplateInfo" });
            }
            var rootNode = TreeNodes.FirstOrDefault();

            // Update or Add TemplateName and OEM under the root node
            UpdateOrAddTemplateNodeUnderRoot(rootNode, "TemplateName", TemplateNameTextBox.Text);
            UpdateOrAddTemplateNodeUnderRoot(rootNode, "OEM", OEMTextBox.Text);

            // SupportedDataBlockTypes node
            TreeNode typesNode = rootNode.Children.FirstOrDefault(node => node.Header.StartsWith("SupportedDataBlockTypes")) ?? new TreeNode { Header = "SupportedDataBlockTypes" };
            // Clear existing types
            typesNode.Children.Clear();
            foreach (var type in TypesComboBox.Items)
            {
                var typeString = type.ToString(); // Make sure this conversion is appropriate for your data
                typesNode.Children.Add(new TreeNode { Header = $"Types: {typeString}" });
            }
            // Ensure SupportedDataBlockTypes is a child of the root node
            if (!rootNode.Children.Contains(typesNode))
            {
                rootNode.Children.Add(typesNode);
            }

            // Communication node
            TreeNode communicationNode = rootNode.Children.FirstOrDefault(node => node.Header.StartsWith("Communication")) ?? new TreeNode { Header = "Communication" };
            // Clear existing communication details
            communicationNode.Children.Clear();
            // Adding child nodes for each communication detail
            communicationNode.Children.Add(new TreeNode { Header = $"CommunicationProtocol: {CProtocolTextbox.Text}" });
            communicationNode.Children.Add(new TreeNode { Header = $"CommunicationDevice: {CDeviceTextbox.Text}" });
            communicationNode.Children.Add(new TreeNode { Header = $"BaudRate: {BaudRateComboBox.Text}" });
            communicationNode.Children.Add(new TreeNode { Header = $"PhysicalCANID: {physicalTextbox.Text}" });
            communicationNode.Children.Add(new TreeNode { Header = $"ResponseCANID: {ResponseTextbox.Text}" });
            communicationNode.Children.Add(new TreeNode { Header = $"FunctionalCANID: {FuntionalTextbox.Text}" });
            // Ensure Communication is a child of the root node
            if (!rootNode.Children.Contains(communicationNode))
            {
                rootNode.Children.Add(communicationNode);
            }
            AddProcedureNodes(rootNode);
        }
        private void AddProcedureNodes(TreeNode rootNode)
        {
            // Define the headers for the child nodes to be added
            var procedures = new[]
            {
            "PreProgrammingProcedure",
            "DataBlockProgrammingProcedure",
            "PostProgrammingProcedure",
            "FailProgrammingProcedure"
            };

            string[] procedureNodes = new string[] { "PreProgrammingProcedure", "DataBlockProgrammingProcedure", "PostProgrammingProcedure", "FailProgrammingProcedure" };
            foreach (string procedureNode in procedureNodes)
            {
                // Check if the procedure node already exists
                TreeNode existingNode = rootNode.Children.FirstOrDefault(n => n.Header == procedureNode);
                if (existingNode == null)
                {
                    // Create the procedure node
                    TreeNode procedureTreeNode = new TreeNode { Header = procedureNode };
                    rootNode.Children.Add(procedureTreeNode);

                    // Add the Services child node to each procedure node
                    //TreeNode servicesNode = new TreeNode { Header = "" };

                    //procedureTreeNode.Children.Add(servicesNode);


                    // Prepare data for AddNewElementsToSelectedItem10
                    List<(string ServiceName, string ServiceType)> serviceDetails = GetServiceDetails();
                    Dictionary<string, string> collectedData = new Dictionary<string, string>();
                    foreach (var detail in serviceDetails)
                    {
                        collectedData.Add(detail.ServiceName, detail.ServiceType);
                    }
                    List<string> groupBoxNames = new List<string> { "Request", "Response" };

                    // Call AddNewElementsToSelectedItem10 for the Services node
                    AddNewElementsToSelectedItem10(collectedData, groupBoxNames);
                }
            }
        }
        public List<(string ServiceName, string ServiceType)> GetServiceDetails()
        {
            // Example implementation: return a list of service details
            // In a real scenario, this method would retrieve data from a database, file, or another source.
            return new List<(string, string)>
            {
               ("Service_Name", "DiagnosticSessionControl"),
               ("ServiceB", "Type2"),
               ("ServiceC", "Type3")
            };
        }

       
        private void UpdateOrAddTemplateNodeUnderRoot(TreeNode rootNode, string label, string value)
        {
            // Check if the node with the given label already exists under the root node
            var targetNode = rootNode.Children.FirstOrDefault(n => n.Header.StartsWith(label + ":"));
            if (targetNode == null)
            {
                // If the node doesn't exist, add a new one
                targetNode = new TreeNode { Header = $"{label}: {value}" };
                rootNode.Children.Add(targetNode);
            }
            else
            {
                // If it exists, update its value
                targetNode.Header = $"{label}: {value}";
            }
        }

        #endregion

        #region GenerateXML
        private XDocument GenerateXml()
        {
            // Start by creating the root element
            XElement templateInfoElement = new XElement("TemplateInfo");

            // Assuming the root node is the first item in the TreeNodes collection
            TreeNode rootNode = TreeNodes.FirstOrDefault();
            if (rootNode != null)
            {
                // Process each child of the root node
                foreach (TreeNode childNode in rootNode.Children)
                {
                    BuildXmlFromNode(childNode, templateInfoElement);
                }
            }

            // Create the XDocument with the root element
            XDocument doc = new XDocument(templateInfoElement);

            return doc;
        }
      


        private void BuildXmlFromNode(TreeNode node, XElement parentElement)
        {
            // Check if the node's header is "Service-10_Diagnostic"
            if (node.Header.Equals("Service-10_Diagnostic", StringComparison.OrdinalIgnoreCase) ||
                 node.Header.Equals("Service-11_ECUReset", StringComparison.OrdinalIgnoreCase) ||
                 node.Header.Equals("Service-27_SecurityAccess", StringComparison.OrdinalIgnoreCase) ||
                 node.Header.Equals("Service-31_RoutineControl", StringComparison.OrdinalIgnoreCase) ||
                 node.Header.Equals("Service-85_CommunicationMessage", StringComparison.OrdinalIgnoreCase) ||
                 node.Header.Equals("Service-28_ConrolDTC", StringComparison.OrdinalIgnoreCase) ||
                 node.Header.Equals("Service-2E_WriteDataByIdentifier", StringComparison.OrdinalIgnoreCase) ||
                 node.Header.Equals("Service-22_ReadDataByIdentifier", StringComparison.OrdinalIgnoreCase) ||
                 node.Header.Equals("Service-34_RequestDownload", StringComparison.OrdinalIgnoreCase)||
                 node.Header.Equals ("Service-36_TransferData", StringComparison.OrdinalIgnoreCase)||
                 node.Header.Equals("Service-37_RequestTransferExit", StringComparison.OrdinalIgnoreCase))
            {
                XElement servicesElement = new XElement("Services");

                
                // Process each child node of this service, adding them to the new <Service> element
                foreach (TreeNode childNode in node.Children)
                {
                    BuildXmlFromNode(childNode, servicesElement);
                }

                // Add the newly created <Services> element to the parent
                parentElement.Add(servicesElement);
            }
            else
            {
                // Split the header to extract the element name and optional value
                string[] headerParts = node.Header.Split(new[] { ':' }, 2);
                string elementName = headerParts[0].Trim();
                string elementValue = (headerParts.Length > 1) ? headerParts[1].Trim() : null;

                XElement nodeElement = new XElement(SanitizeXmlName(elementName));
                if (!string.IsNullOrEmpty(elementValue))
                {
                    nodeElement.Value = elementValue;
                }

                parentElement.Add(nodeElement);

                foreach (TreeNode childNode in node.Children)
                {
                    BuildXmlFromNode(childNode, nodeElement);
                }
            }
        }

        
        private string SanitizeXmlName(string name)
        {
            // This method should remove invalid characters and return a valid XML name
            return name.Replace(" ", "").Replace("-", "");
        }



        //private XElement CreateXmlElement(string elementName, string elementValue)
        //{
        //    // Create a new XElement with name and value, consider any necessary processing for values
        //    return new XElement(elementName, elementValue);
        //}






        //private XElement CreateXmlElement(string name, string value)
        //{
        //    // Ensure that XML element names are compliant by checking if the name starts with a digit
        //    if (char.IsDigit(name[0]))
        //    {
        //        // Use a generic but compliant element name and store the actual name in an attribute
        //        return new XElement("Element", new XAttribute("name", name), value);
        //    }
        //    else
        //    {
        //        // Directly use the name if it is compliant
        //        return new XElement(name, value);
        //    }
        //}


        //private string SanitizeXmlName(string name)
        //{
        //    // First, convert words representing digits to actual digits
        //    name = ConvertWordToDigit(name);

        //    // If the name starts with a digit after conversion, prepend an underscore to make it a valid XML name
        //    if (char.IsDigit(name[0]))
        //    {
        //        name = "_" + name;
        //    }

        //    // Replace any characters that are not valid in XML names with underscores.
        //    // Valid characters are letters, digits, hyphens, and underscores.
        //    // Spaces and other invalid characters are replaced by underscores.
        //    return new string(name.Select(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_').ToArray());
        //}

        private string ConvertWordToDigit(string name)
        {
            // Mapping words to digits
            Dictionary<string, char> wordToDigit = new Dictionary<string, char>
            {
               { "Zero", '0'}, { "Two", '2'}, { "Four", '4'}, { "Eight", '8'}
            };

            // Check if the beginning of the name matches any word in the dictionary
            foreach (var entry in wordToDigit)
            {
                if (name.StartsWith(entry.Key))
                {
                    // Replace the word with its corresponding digit and return the modified name
                    return entry.Value + name.Substring(entry.Key.Length);
                }
            }

            // Return the original name if no match is found
            return name;
        }










        private void DisplayXml(XDocument xml)
        {
            XmlRichTextBox.Document.Blocks.Clear();
            XmlRichTextBox.Document.Blocks.Add(new Paragraph(new Run(xml.ToString())));
        }

        #endregion

        #region RemoveTypes
        private void RemoveTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {

            // Get the selected TreeView item
            var selectedItem = treeView.SelectedItem as TreeNode;
            if (selectedItem == null) return;

            // Check if the selected item is a "Types" node
            if (selectedItem.Header.StartsWith("Types:"))
            {
                // Remove the type from the TreeView
                var parentNode = FindParentNode(selectedItem);
                if (parentNode != null)
                {
                    parentNode.Children.Remove(selectedItem);
                }

                // Remove the type from the XML structure
                RemoveTypeFromXml(selectedItem.Header);

                // Remove the type from the TypesComboBox
                var typeToRemove = selectedItem.Header.Split(':')[1].Trim();
                var comboBoxItem = TypesComboBox.Items.Cast<string>().FirstOrDefault(item => item == typeToRemove);
                if (comboBoxItem != null)
                {
                    TypesComboBox.Items.Remove(comboBoxItem);
                }
            }
        }
        private TreeNode FindParentNode(TreeNode node)
        {
            foreach (var rootNode in TreeNodes)
            {
                var parentNode = FindParentNodeRecursive(rootNode, node);
                if (parentNode != null)
                {
                    return parentNode;
                }
            }
            return null;
        }

        private TreeNode FindParentNodeRecursive(TreeNode parent, TreeNode child)
        {
            foreach (var childNode in parent.Children)
            {
                if (childNode == child)
                {
                    return parent;
                }
                var result = FindParentNodeRecursive(childNode, child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private void RemoveTypeFromXml(string typeHeader)
        {
            if (_currentXml == null || string.IsNullOrWhiteSpace(typeHeader)) return;

            // Assuming the header follows the format "Types: TypeName"
            string[] headerParts = typeHeader.Split(':');
            if (headerParts.Length < 2) return; // Early return if the header does not contain the expected format

            string typeName = headerParts[1].Trim();
            if (string.IsNullOrWhiteSpace(typeName)) return; // Additional check for empty typeName

            var supportedDataBlockTypesNode = _currentXml.Root.Element("SupportedDataBlockTypes");
            if (supportedDataBlockTypesNode == null) return;

            var typesToRemove = supportedDataBlockTypesNode.Elements("Types")
                                .Where(e => e.Value.Trim().Equals(typeName, StringComparison.OrdinalIgnoreCase))
                                .ToList();

            foreach (var typeElement in typesToRemove)
            {
                typeElement.Remove();
            }
        }

        #endregion

        #region MouseClicks


        //private void TreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    // Get the clicked item
        //    var hit = VisualTreeHelper.HitTest(treeView, Mouse.GetPosition(treeView));
        //    var clickedItem = hit.VisualHit.GetParentOfType<TreeViewItem>();

        //    if (clickedItem != null)
        //    {
        //        // Check if the clicked item is the "Types" node
        //        var node = clickedItem.DataContext as TreeNode;
        //        if (node != null)
        //        {
        //            if (node.Header.StartsWith("Types:"))
        //            {
        //                clickedItem.ContextMenu = (ContextMenu)treeView.FindResource("TypesContextMenu");
        //            }
        //            else if (node.Header.Equals("PreProgrammingProcedure")|| node.Header.Equals("DataBlockProgrammingProcedure"))
        //            {
        //                clickedItem.ContextMenu = (ContextMenu)this.Resources["ServicesContextMenu"];


        //            }
        //            else
        //            {
        //                // Clear the context menu for other items
        //                clickedItem.ContextMenu = null;
        //            }
        //        }
        //    }
        //}
        private void SetupTreeNode(TreeNode node, TreeViewItem clickedItem)
        {
            if (node.Header.StartsWith("Types:"))
            {
                clickedItem.ContextMenu = (ContextMenu)treeView.FindResource("TypesContextMenu");
            }
            else if (node.Header.Equals("PreProgrammingProcedure") || node.Header.Equals("DataBlockProgrammingProcedure"))
            {
                
                clickedItem.ContextMenu = (ContextMenu)this.Resources["ServicesContextMenu"];
            }
            else
            {
                clickedItem.ContextMenu = null;
            }
        }

        private void TreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var hit = VisualTreeHelper.HitTest(treeView, Mouse.GetPosition(treeView));
            var clickedItem = hit.VisualHit.GetParentOfType<TreeViewItem>();

            if (clickedItem != null && clickedItem.DataContext is TreeNode node)
            {
                SetupTreeNode(node, clickedItem);
            }
        }





        //private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    // Use VisualUpwardSearch to find the TreeViewItem that was double-clicked
        //    var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject);

        //    if (treeViewItem != null && treeViewItem.DataContext is TreeNode treeNode)
        //    {

        //        if (lastSelectedServiceType == "Service-10_Diagnostic" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            Service_10 window = new Service_10(_selectedTreeViewItem, treeView, treeNode);
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode(treeNode);
        //                window.LoadExistingData(existingData);

        //            }

        //            window.DataRetrieved += UpdateTreeView;
        //            window.Show();
        //        }



        //        else if (lastSelectedServiceType == "Service-11_ECUReset" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            Service_11 window = new Service_11(_selectedTreeViewItem, treeView, treeNode);
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode(treeNode);
        //                window.LoadExistingData11(existingData);

        //            }
        //            window.DataRetrieved += UpdateTreeView;
        //            window.Show();
        //        }

        //        else if (lastSelectedServiceType == "Service-27_SecurityAccess" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            // Assuming treeViewItem is already defined in the scope as a TreeViewItem
        //            _selectedTreeViewItem = treeViewItem;

        //            // Create a new instance of Srvice27Security (note: check spelling, might be 'Service27Security')
        //            Srvice27Security window = new Srvice27Security(); // Assuming the constructor can take these parameters

        //            // Set the window title to the node's header
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode27(treeNode);
        //                window.LoadExistingData27(existingData);

        //            }
        //            window.DataRetrieved += UpdateTreeView;
        //            // Show the window
        //            window.Show();
        //        }

        //        else if (lastSelectedServiceType == "Service-31_RoutineControl" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            Service31 window = new Service31();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode31(treeNode);
        //                // Assume LoadExistingData31 is a method you need to call here, uncomment when ready
        //                window.LoadExistingData31(existingData);
        //            }
        //            window.Show();
        //        }


        //        else if (lastSelectedServiceType == "Service-28_ConrolDTC" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            Service28 window = new Service28();
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode(treeNode);
        //                window.LoadExistingData28(existingData);

        //            }
        //            window.DataRetrieved += UpdateTreeView;
        //            window.Show();
        //        }
        //        else if (lastSelectedServiceType == "Service-2E_WriteDataByIdentifier" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            Service2e window = new Service2e();
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode(treeNode);
        //                window.LoadExistingData2E(existingData);

        //            }

        //            window.DataRetrieved += UpdateTreeView;
        //            window.Show();
        //        }

        //        else if (lastSelectedServiceType == "Service-85_CommunicationMessage" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            ControlDTC window = new ControlDTC();
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode(treeNode);
        //                window.LoadExistingData85(existingData);

        //            }
        //            window.Show();
        //        }

        //        else if (lastSelectedServiceType == "Service-22_ReadDataByIdentifier" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            Service_22 window = new Service_22();
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode22(treeNode);
        //                window.LoadExistingData22(existingData);

        //            }

        //            window.DataRetrieved += UpdateTreeView;
        //            window.Show();
        //        }
        //        else if (lastSelectedServiceType == "Service-34_RequestDownload" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            Service_34 window = new Service_34();
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode(treeNode);
        //                window.LoadExistingData34(existingData);

        //            }
        //            window.Show();
        //        }
        //        else if (lastSelectedServiceType == "Service-36_TransferData" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            Service_36 window = new Service_36();
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode(treeNode);
        //                window.LoadExistingData36(existingData);

        //            }
        //            window.Show();
        //        }
        //        else if (lastSelectedServiceType == "Service-37_RequestTransferExit" && treeNode.Header.ToString().Equals("Services"))
        //        {
        //            _selectedTreeViewItem = treeViewItem;
        //            Service_37 window = new Service_37();
        //            window.Title = treeNode.Header.ToString();
        //            window.IsEditMode = treeNode.Children.Count > 0;
        //            if (window.IsEditMode)
        //            {
        //                var existingData = ExtractDataFromNode(treeNode);
        //                window.LoadExistingData37(existingData);

        //            }
        //            window.Show();
        //        }

        //        else
        //        {
        //            // If the DataContext is not a TreeNode, you might want to handle this case differently
        //            // For example, you could log a message or perform some other action
        //            Debug.WriteLine("The DataContext of the TreeViewItem is not a TreeNode.");
        //        }
        //    }
        //    else
        //    {
        //        // If no TreeViewItem was found, you might want to handle this case differently
        //        // For example, you could log a message or perform some other action
        //        Debug.WriteLine("No TreeViewItem was found.");
        //    }


        //}
        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Only respond to the left button double-click to prevent unwanted behavior
            if (e.ChangedButton != MouseButton.Left)
                return;

            // Use VisualUpwardSearch to find the TreeViewItem that was double-clicked
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject);

            if (treeViewItem != null && treeViewItem.DataContext is TreeNode treeNode)
            {
                if (treeNode.Header.ToString().Equals("Services"))
                {
                    // Only open the window if the correct service type is selected
                    if (lastSelectedServiceType == "Service-10_Diagnostic")
                    {
                        OpenService10Window(treeNode, treeViewItem);
                    }
                    else if(lastSelectedServiceType == "Service-11_ECUReset")
                    {
                        OpenService11Window(treeNode, treeViewItem);
                    }
                    else if(lastSelectedServiceType == "Service-27_SecurityAccess")
                    {
                        OpenService27Window(treeNode, treeViewItem);
                    }
                    else if(lastSelectedServiceType == "Service-31_RoutineControl")
                    {
                        OpenService31Window(treeNode, treeViewItem);
                    }
                    else if (lastSelectedServiceType == "Service-28_ConrolDTC")
                    {
                        OpenService28Window(treeNode, treeViewItem);
                    }
                    else if (lastSelectedServiceType == "Service-2E_WriteDataByIdentifier")
                    {
                        OpenService2EWindow(treeNode, treeViewItem);
                    }
                    else if (lastSelectedServiceType == "Service-85_CommunicationMessage")
                    {
                        OpenService85Window(treeNode, treeViewItem);
                    }
                    else if (lastSelectedServiceType == "Service-22_ReadDataByIdentifier")
                    {
                        OpenService22Window(treeNode, treeViewItem);
                    }
                    else if (lastSelectedServiceType == "Service-34_RequestDownload")
                    {
                        OpenService34Window(treeNode, treeViewItem);
                    }
                    else if (lastSelectedServiceType == "Service-36_TransferData")
                    {
                        OpenService36Window(treeNode, treeViewItem);
                    }
                    else if(lastSelectedServiceType== "Service-37_RequestTransferExit")
                    {
                        OpenService37Window(treeNode, treeViewItem);
                    }
                    // Mark the event as handled to prevent it from firing again
                    e.Handled = true;
                }
            }
            else
            {
                // If no TreeViewItem was found or not the right data context, log this
                Debug.WriteLine("No TreeViewItem was found or the DataContext is not a TreeNode.");
            }
        }

        private void OpenService10Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            Service_10 window = new Service_10(_selectedTreeViewItem, treeView, treeNode);
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode(treeNode);
                window.LoadExistingData(existingData);
            }
            window.DataRetrieved += UpdateTreeView;
            window.Show();
        }
        private void OpenService11Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            Service_11 window = new Service_11(_selectedTreeViewItem, treeView, treeNode);
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode(treeNode);
                window.LoadExistingData11(existingData);
            }
            window.DataRetrieved += UpdateTreeView;
            window.Show();
        }
        private void OpenService27Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            // Assuming treeViewItem is already defined in the scope as a TreeViewItem
            _selectedTreeViewItem = treeViewItem;

            // Create a new instance of Srvice27Security (note: check spelling, might be 'Service27Security')
            Srvice27Security window = new Srvice27Security(); // Assuming the constructor can take these parameters

            // Set the window title to the node's header
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode27(treeNode);
                window.LoadExistingData27(existingData);

            }
            window.DataRetrieved += UpdateTreeView;
            window.Show();
        }
        private void OpenService31Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            Service31 window = new Service31();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode31(treeNode);
                // Assume LoadExistingData31 is a method you need to call here, uncomment when ready
                window.LoadExistingData31(existingData);
            }
            window.Show();
        }
        private void OpenService28Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            Service28 window = new Service28();
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode(treeNode);
                window.LoadExistingData28(existingData);

            }
            window.DataRetrieved += UpdateTreeView;
            window.Show();
        }
        private void OpenService2EWindow(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            Service2e window = new Service2e();
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode(treeNode);
                window.LoadExistingData2E(existingData);

            }

            window.DataRetrieved += UpdateTreeView;
            window.Show();
        }
        private void OpenService85Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            ControlDTC window = new ControlDTC();
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode(treeNode);
                window.LoadExistingData85(existingData);

            }
            window.Show();
        }
        private void OpenService22Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            Service_22 window = new Service_22();
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode22(treeNode);
                window.LoadExistingData22(existingData);

            }

            window.DataRetrieved += UpdateTreeView;
            window.Show();
        }
        private void OpenService34Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            Service_34 window = new Service_34();
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode(treeNode);
                window.LoadExistingData34(existingData);

            }
            window.Show();
        }
        private void OpenService36Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            Service_36 window = new Service_36();
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode(treeNode);
                window.LoadExistingData36(existingData);

            }
            window.Show();
        }
        private void OpenService37Window(TreeNode treeNode, TreeViewItem treeViewItem)
        {
            _selectedTreeViewItem = treeViewItem;
            Service_37 window = new Service_37();
            window.Title = treeNode.Header.ToString();
            window.IsEditMode = treeNode.Children.Count > 0;
            if (window.IsEditMode)
            {
                var existingData = ExtractDataFromNode(treeNode);
                window.LoadExistingData37(existingData);

            }
            window.Show();
        }
        // Additional helper methods needed for the above code
        private void UpdateTreeView(object sender, EventArgs e)
        {
            treeView.Items.Refresh();
        }





        private void RemoveNodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Ensure this method only removes the selected node
            var selectedItem = treeView.SelectedItem as TreeNode;
            if (selectedItem == null) return;

            var parentNode = FindParentNode(selectedItem);
            if (parentNode != null)
            {
                parentNode.Children.Remove(selectedItem);
            }
            else
            {
                TreeNodes.Remove(selectedItem);
            }

            treeView.Items.Refresh();
        }

        private string GetStandardGroupName(string groupBoxName)
        {
            var nameMap = new Dictionary<string, string>
          {
        {"Request1", "Request"},
        {"Response1", "Response"},
        {"Request2", "Request"},
        {"Response2", "Response"}
           };

            if (nameMap.ContainsKey(groupBoxName))
                return nameMap[groupBoxName];

            return groupBoxName; // Return the original if no mapping is found
        }


        private void ServiceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            // Check if the selected item is a TreeViewItem and get the TreeNode from its DataContext

            if (menuItem != null && treeView.SelectedItem is TreeNode selectedNode &&
              (selectedNode.Header.Equals("PreProgrammingProcedure") ||
               selectedNode.Header.Equals("DataBlockProgrammingProcedure") ||
               selectedNode.Header.Equals("PostProgrammingProcedure")||
               selectedNode.Header.Equals("FailProgrammingProcedure")))
            {
                selectedNode.Children.Add(new TreeNode { Header = "Services" });
                lastSelectedServiceType = menuItem.Header.ToString();

                // Save the last selected service type in settings
                Properties.Settings.Default.AllSelectedServiceType = lastSelectedServiceType;
                Properties.Settings.Default.Save();
            }

           

        }


        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeViewItem = sender as TreeViewItem;
            if (treeViewItem != null && treeViewItem.DataContext is TreeNode treeNode)
            {
                _selectedTreeViewItem = treeViewItem;
            }
        }



        private TreeNode FindParent(TreeNode child, TreeNode root)
        {
            if (root.Children.Contains(child))
            {
                return root;
            }
            foreach (var node in root.Children)
            {
                var result = FindParent(child, node);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        #endregion

        #region ExtractDataFromNode
        private Dictionary<string, string> ExtractDataFromNode(TreeNode node)
        {
            var collectedData = new Dictionary<string, string>();

            // Helper method to process a node and its children recursively
            void ProcessNode(TreeNode currentNode)
            {
                // Process each child node of the current node
                foreach (var childNode in currentNode.Children)
                {
                    // Assuming each child node's Header is structured as "Key: Value"
                    var childHeaderParts = childNode.Header.Split(new[] { ':' }, 2);

                    if (childHeaderParts.Length == 2)
                    {
                        string key = childHeaderParts[0].Trim();
                        string value = childHeaderParts[1].Trim();
                        
                        // Add the extracted key-value pair to the collectedData dictionary
                        collectedData[key] = value;
                    }

                    if (currentNode.Header.Equals("Request"))
                    {

                        // Check if the current node is "OptionalBytes" or one of its children
                        if (currentNode.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase))
                        {
                            // Handle "OptionalBytes" and its children differently
                            Console.WriteLine($"Processing special node: {currentNode.Header}");
                            // Extract data from the child nodes of "OptionalBytes"
                            foreach (var optionalBytesChildNode in currentNode.Children)
                            {
                                if (optionalBytesChildNode.Header.StartsWith("Used"))
                                {
                                    // Assuming "Used" is always true when OptionalBytes is checked
                                    collectedData["Used"] = "True";
                                }
                                int indexCounter = 0;
                                int valueCounter = 0;
                                foreach (var byteChildNode in optionalBytesChildNode.Children)
                                {


                                    // Extract "Index" and "Value" from the child nodes of "OptionalBytes"
                                    if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Extract the index value and store it in collectedData with a unique key
                                        collectedData[$"Index{indexCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                        indexCounter++;
                                    }
                                    else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                                    {
                                        collectedData[$"Value{valueCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                        valueCounter++;
                                    }

                                }
                            }
                            continue;
                        }


                        

                    }

                    if (childNode.Header.Equals("Response"))
                    {
                        foreach (var RspChildNode in childNode.Children)
                            // Check if the current node is "OptionalBytes" or one of its children
                            if (RspChildNode.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase))
                            {
                                // Handle "OptionalBytes" and its children differently
                                Console.WriteLine($"Processing special node: {RspChildNode.Header}");
                                // Extract data from the child nodes of "OptionalBytes"
                                foreach (var optionalBytesChildNode in RspChildNode.Children)
                                {
                                    if (optionalBytesChildNode.Header.StartsWith("Used"))
                                    {
                                        // Assuming "Used" is always true when OptionalBytes is checked
                                        collectedData["Used1"] = "True";
                                    }
                                    int indexCounter = 0;
                                    int valueCounter = 0;
                                    foreach (var byteChildNode in optionalBytesChildNode.Children)
                                    {
                                        // Extract "Index" and "Value" from the child nodes of "OptionalBytes"
                                        if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                                        {
                                            // Extract the index value and store it in collectedData with a unique key
                                            collectedData[$"index1{indexCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                            indexCounter++;
                                        }
                                        else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                                        {
                                            collectedData[$"value1{valueCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                            valueCounter++;
                                        }
                                    }
                                }

                            }
                        continue;
                    }


                    // Recursively process the child node's children
                    ProcessNode(childNode);
                }
            }

            // Start processing from the given node
            ProcessNode(node);

            return collectedData;
        }
        private Dictionary<string, string> ExtractDataFromNode27(TreeNode node)
        {
            var collectedData = new Dictionary<string, string>();
            int requestCounter = 0;
            int responseCounter = 0;

            void ProcessNode(TreeNode currentNode, string currentContextSuffix = "")
            {
                foreach (var childNode in currentNode.Children)
                {
                    var childHeaderParts = childNode.Header.Split(new[] { ':' }, 2);
                    if (childHeaderParts.Length == 2)
                    {
                        string key = childHeaderParts[0].Trim();
                        string value = childHeaderParts[1].Trim();
                        collectedData[key + currentContextSuffix] = value; // Append suffix to make key unique
                    }

                    // Detect "Request" and "Response" headers to properly handle each instance
                    if (childNode.Header.Equals("Request"))
                    {
                        requestCounter++;
                        string requestPrefix = $"Request{requestCounter}";
                        ProcessNode(childNode, requestPrefix); // Recursive call with new prefix
                        continue;
                    }
                    else if (childNode.Header.Equals("Response"))
                    {
                        responseCounter++;
                        string responsePrefix = $"Response{responseCounter}";
                        ProcessNode(childNode, responsePrefix); // Recursive call with new prefix
                        continue;
                    }

                    // Special handling for "OptionalBytes" within either "Request" or "Response"
                    if (childNode.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase))
                    {
                        ProcessOptionalBytes(childNode, currentContextSuffix, collectedData);
                    }
                    else
                    {
                        // Continue recursively processing without special handling
                        ProcessNode(childNode, currentContextSuffix);
                    }
                }
            }

            // Handles special cases for "OptionalBytes" nodes
            void ProcessOptionalBytes(TreeNode parentNode, string parentPrefix, Dictionary<string, string> data)
            {
                foreach (var childNode in parentNode.Children)
                {
                    string keyPrefix = parentPrefix + childNode.Header; // Unique prefix for keys
                    if (childNode.Header.StartsWith("Used", StringComparison.OrdinalIgnoreCase))
                    {
                        data[keyPrefix + "Used"] = "True";
                    }
                    foreach (var byteChildNode in childNode.Children)
                    {
                        if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                        {
                            var parts = byteChildNode.Header.Split(new[] { ':' }, 2);
                            if (parts.Length == 2)
                            {
                                data[keyPrefix + "Index"] = parts[1].Trim();
                            }
                        }
                        else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                        {
                            var parts = byteChildNode.Header.Split(new[] { ':' }, 2);
                            if (parts.Length == 2)
                            {
                                data[keyPrefix + "Value"] = parts[1].Trim();
                            }
                        }
                    }

                }
            }

            // Initialize processing with the root node
            ProcessNode(node);
            return collectedData;
        }


        private Dictionary<string, string> ExtractDataFromNode22(TreeNode node)
        {
            var collectedData = new Dictionary<string, string>();

            // Helper method to process a node and its children recursively
            void ProcessNode(TreeNode currentNode)
            {
                // Process each child node of the current node
                foreach (var childNode in currentNode.Children)
                {
                    // Assuming each child node's Header is structured as "Key: Value"
                    var childHeaderParts = childNode.Header.Split(new[] { ':' }, 2);

                    if (childHeaderParts.Length == 2)
                    {
                        string key = childHeaderParts[0].Trim();
                        string value = childHeaderParts[1].Trim();

                        // Add the extracted key-value pair to the collectedData dictionary
                        collectedData[key] = value;
                    }

                    if (currentNode.Header.Equals("Request"))
                    {
                        // Check if the current node is "OptionalBytes" or one of its children
                        if (childNode.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase))
                        {
                            // Handle "OptionalBytes" and its children differently
                            Console.WriteLine($"Processing special node: {childNode.Header}");
                            // Extract data from the child nodes of "OptionalBytes"
                            foreach (var optionalBytesChildNode in childNode.Children)
                            {
                                if (optionalBytesChildNode.Header.StartsWith("Used"))
                                {
                                    // Assuming "Used" is always true when OptionalBytes is checked
                                    collectedData["Used"] = "True";
                                }
                                int indexCounter = 0;
                                int valueCounter = 0;
                                foreach (var byteChildNode in optionalBytesChildNode.Children)
                                {


                                    // Extract "Index" and "Value" from the child nodes of "OptionalBytes"
                                    if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Extract the index value and store it in collectedData with a unique key
                                        collectedData[$"Index{indexCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                        indexCounter++;
                                    }
                                    else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                                    {
                                        collectedData[$"Value{valueCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                        valueCounter++;
                                    }

                                }
                            }
                            continue;
                        }

                    }

                    if (currentNode.Header.Equals("Response"))
                    {
                        foreach (var RspChildNode in currentNode.Children)
                            // Check if the current node is "OptionalBytes" or one of its children
                            if (RspChildNode.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase))
                            {
                                // Handle "OptionalBytes" and its children differently
                                Console.WriteLine($"Processing special node: {RspChildNode.Header}");
                                // Extract data from the child nodes of "OptionalBytes"
                                foreach (var optionalBytesChildNode in RspChildNode.Children)
                                {
                                    if (optionalBytesChildNode.Header.StartsWith("Used"))
                                    {
                                        // Assuming "Used" is always true when OptionalBytes is checked
                                        collectedData["Used1"] = "True";
                                    }
                                    int indexCounter = 0;
                                    int valueCounter = 0;
                                    foreach (var byteChildNode in optionalBytesChildNode.Children)
                                    {
                                        // Extract "Index" and "Value" from the child nodes of "OptionalBytes"
                                        if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                                        {
                                            // Extract the index value and store it in collectedData with a unique key
                                            collectedData[$"Index1{indexCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                            indexCounter++;
                                        }
                                        else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                                        {
                                            collectedData[$"Value1{valueCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                            valueCounter++;
                                        }
                                    }
                                }

                            }
                        continue;
                    }


                    // Recursively process the child node's children
                    ProcessNode(childNode);
                }
            }

            // Start processing from the given node
            ProcessNode(node);

            return collectedData;
        }

        private Dictionary<string, string> ExtractDataFromNode31(TreeNode node)
        {
            var collectedData = new Dictionary<string, string>();

            // Helper method to process a node and its children recursively
            void ProcessNode(TreeNode currentNode)
            {
                // Process each child node of the current node
                foreach (var childNode in currentNode.Children)
                {
                    // Assuming each child node's Header is structured as "Key: Value"
                    var childHeaderParts = childNode.Header.Split(new[] { ':' }, 2);

                    if (childHeaderParts.Length == 2)
                    {
                        string key = childHeaderParts[0].Trim();
                        string value = childHeaderParts[1].Trim();

                        // Add the extracted key-value pair to the collectedData dictionary
                        collectedData[key] = value;
                    }

                    if (currentNode.Header.Equals("Request"))
                    {
                        foreach (var childNodes in currentNode.Children)
                        {
                            // Check if the current node is "OptionalBytes" or one of its children
                            if (childNodes.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase))
                            {
                                // Handle "OptionalBytes" and its children differently
                                Console.WriteLine($"Processing special node: {childNodes.Header}");
                                // Extract data from the child nodes of "OptionalBytes"
                                foreach (var optionalBytesChildNode in childNodes.Children)
                                {
                                    if (optionalBytesChildNode.Header.StartsWith("Used"))
                                    {
                                        // Assuming "Used" is always true when OptionalBytes is checked
                                        collectedData["Used"] = "True";
                                    }
                                    int indexCounter = 0;
                                    int valueCounter = 0;
                                    foreach (var byteChildNode in optionalBytesChildNode.Children)
                                    {


                                        // Extract "Index" and "Value" from the child nodes of "OptionalBytes"
                                        if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                                        {
                                            // Extract the index value and store it in collectedData with a unique key
                                            collectedData[$"Index{indexCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                            indexCounter++;
                                        }
                                        else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                                        {
                                            collectedData[$"Value{valueCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                            valueCounter++;
                                        }

                                    }
                                }
                                continue;
                            }
                            else if (childNodes.Header.Equals("RoutineControlOptionRecord", StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (var routineChildNode in childNodes.Children)
                                {
                                    if (routineChildNode.Header.Equals("OptionalBytes"))
                                    {
                                        Console.WriteLine($"Processing special node: {routineChildNode.Header}");
                                        // Extract data from the child nodes of "OptionalBytes"
                                        foreach (var optionalBytesChildNode in routineChildNode.Children)
                                        {
                                            if (optionalBytesChildNode.Header.StartsWith("Used"))
                                            {
                                                // Assuming "Used" is always true when OptionalBytes is checked
                                                collectedData["Used4"] = "True";
                                            }
                                            int indexCounter = 0;
                                            int valueCounter = 0;
                                            foreach (var byteChildNode in optionalBytesChildNode.Children)
                                            {


                                                // Extract "Index" and "Value" from the child nodes of "OptionalBytes"
                                                if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    // Extract the index value and store it in collectedData with a unique key
                                                    collectedData[$"Index4{indexCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                                    indexCounter++;
                                                }
                                                else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    collectedData[$"Value4{valueCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                                    valueCounter++;
                                                }

                                            }
                                        }
                                        continue;
                                    }

                                }
                                // Handle "OptionalBytes" and its children differently

                            }

                        }
                    }
                        //if()
                        //{

                        //}

                    

                    if (childNode.Header.Equals("Response"))
                    {

                        foreach (var RspChildNode in childNode.Children)
                        {
                            if(RspChildNode.Header.Equals("RoutineInfo", StringComparison.OrdinalIgnoreCase))
                            {
                                foreach(var ChildNode in RspChildNode.Children)
                                {
                                    if (ChildNode.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Handle "OptionalBytes" and its children differently
                                        Console.WriteLine($"Processing special node: {RspChildNode.Header}");
                                        // Extract data from the child nodes of "OptionalBytes"
                                        foreach (var optionalBytesChildNode in ChildNode.Children)
                                        {
                                            if (optionalBytesChildNode.Header.StartsWith("Used"))
                                            {
                                                // Assuming "Used" is always true when OptionalBytes is checked
                                                collectedData["Used1"] = "True";
                                            }
                                            int indexCounter = 0;
                                            int valueCounter = 0;
                                            foreach (var byteChildNode in optionalBytesChildNode.Children)
                                            {
                                                // Extract "Index" and "Value" from the child nodes of "OptionalBytes"
                                                if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    // Extract the index value and store it in collectedData with a unique key
                                                    collectedData[$"index1{indexCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                                    indexCounter++;
                                                }
                                                else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    collectedData[$"value1{valueCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                                    valueCounter++;
                                                }
                                            }
                                        }

                                    }
                                    continue;
                                }
                                
                            }
                            else if(RspChildNode.Header.Equals("RoutineStatusRecord", StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (var ChildNode in RspChildNode.Children)
                                {
                                    if (ChildNode.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Handle "OptionalBytes" and its children differently
                                        Console.WriteLine($"Processing special node: {RspChildNode.Header}");
                                        // Extract data from the child nodes of "OptionalBytes"
                                        foreach (var optionalBytesChildNode in ChildNode.Children)
                                        {
                                            if (optionalBytesChildNode.Header.StartsWith("Used"))
                                            {
                                                // Assuming "Used" is always true when OptionalBytes is checked
                                                collectedData["Used2"] = "True";
                                            }
                                            int indexCounter = 0;
                                            int valueCounter = 0;
                                            foreach (var byteChildNode in optionalBytesChildNode.Children)
                                            {
                                                // Extract "Index" and "Value" from the child nodes of "OptionalBytes"
                                                if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    // Extract the index value and store it in collectedData with a unique key
                                                    collectedData[$"index2{indexCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                                    indexCounter++;
                                                }
                                                else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    collectedData[$"value2{valueCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                                    valueCounter++;
                                                }
                                            }
                                        }

                                    }
                                    continue;
                                }

                            }
                            else if (RspChildNode.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase) && !RspChildNode.Header.Equals("RoutineInfo", StringComparison.OrdinalIgnoreCase) && !RspChildNode.Header.Equals("RoutineStatusRecord", StringComparison.OrdinalIgnoreCase))
                            {
                                if (RspChildNode.Header.Equals("OptionalBytes", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Handle "OptionalBytes" and its children differently
                                    Console.WriteLine($"Processing special node: {RspChildNode.Header}");
                                    // Extract data from the child nodes of "OptionalBytes"
                                    foreach (var optionalBytesChildNode in RspChildNode.Children)
                                    {
                                        if (optionalBytesChildNode.Header.StartsWith("Used"))
                                        {
                                            // Assuming "Used" is always true when OptionalBytes is checked
                                            collectedData["Used3"] = "True";
                                        }
                                        int indexCounter = 0;
                                        int valueCounter = 0;
                                        foreach (var byteChildNode in optionalBytesChildNode.Children)
                                        {
                                            // Extract "Index" and "Value" from the child nodes of "OptionalBytes"
                                            if (byteChildNode.Header.StartsWith("Index", StringComparison.OrdinalIgnoreCase))
                                            {
                                                // Extract the index value and store it in collectedData with a unique key
                                                collectedData[$"index3{indexCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                                indexCounter++;
                                            }
                                            else if (byteChildNode.Header.StartsWith("Value", StringComparison.OrdinalIgnoreCase))
                                            {
                                                collectedData[$"value3{valueCounter}"] = byteChildNode.Header.Split(new[] { ':' }, 2)[1].Trim();
                                                valueCounter++;
                                            }
                                        }
                                    }

                                }
                                continue;
                            }


                        }
                                // Check if the current node is "OptionalBytes" or one of its children

                           
                        
                        
                        
                    }


                    // Recursively process the child node's children
                    ProcessNode(childNode);
                }
            }

            // Start processing from the given node
            ProcessNode(node);

            return collectedData;
        }


        #endregion

        #region UpdateTreeview
        public void AddNewElementsToSelectedItem11(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!item.Key.StartsWith("OptionalBytes") && !item.Key.StartsWith("ResetType") && !item.Key.StartsWith("ResetTypeName") && !item.Key.StartsWith("SuppressPositiveResponse") && !item.Key.StartsWith("SessionParameterRecord"))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && (item.Key == "ResetType" || item.Key == "ResetTypeName" || item.Key == "SuppressPositiveResponse"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (isResponseGroup && (item.Key == "ResetType1" || item.Key == "PowerDownTime"))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }
                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes"))
                                {
                                    string usedValue = collectedData["OptionalBytes"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}";
                                            string valueKey = $"OptionalBytes_Value{i}";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index1{i}";
                                        string valueKey = $"OptionalBytes_Value1{i}";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }


                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh();
                        
                    }
                }
            }
        }

        public void AddNewElementsToSelectedItem10(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!item.Key.StartsWith("OptionalBytes") && !item.Key.StartsWith("SessionType") && !item.Key.StartsWith("SessionName") && !item.Key.StartsWith("SuppressPositiveResponse") && !item.Key.StartsWith("SessionParameterRecord"))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && (item.Key == "SessionType" || item.Key == "SessionName" || item.Key == "SuppressPositiveResponse"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (isResponseGroup && (item.Key == "SessionType1" || item.Key == "SessionParameterRecord"))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }
                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes"))
                                {
                                    string usedValue = collectedData["OptionalBytes"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        //Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}";
                                            string valueKey = $"OptionalBytes_Value{i}";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index1{i}";
                                        string valueKey = $"OptionalBytes_Value1{i}";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }
                        

                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh(); // This line might not be necessary

                        //this.Close();
                    }
                }
            }
        }

        // This method now returns a tuple: the normalized key name and the numeric suffix
        private (string NormalizedKey, string NumericSuffix) NormalizeKeyName(string key)
        {
            var match = Regex.Match(key, @"(\D+)(\d*)$");
            return (match.Groups[1].Value, match.Groups[2].Value);
        }


        public void AddNewElementsToSelectedItem28(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!item.Key.StartsWith("OptionalBytes") && !item.Key.StartsWith("SessionType") && !item.Key.StartsWith("Session_Name") && !item.Key.StartsWith("SuppressPositiveResponse"))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && (item.Key == "SessionType" || item.Key == "Session_Name" || item.Key == "SuppressPositiveResponse"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (isResponseGroup && (item.Key == "SessionType1" || item.Key == "SessionParameterRecord"))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }
                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes_State_Request1"))
                                {
                                    string usedValue = collectedData["OptionalBytes_State_Request1"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}_Request1";
                                            string valueKey = $"OptionalBytes_Value{i}_Request1";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index1{i}";
                                        string valueKey = $"OptionalBytes_Value1{i}";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }


                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh();
                    }
                }
            }
        }
        public void AddNewElementsToSelectedItem27(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        var (normalizedKey, numericSuffix) = NormalizeKeyName(item.Key);

                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if ((normalizedKey.Trim() != "Requesting") && (normalizedKey.Trim() != "SuppresssPositiveResponse")&& (normalizedKey.Trim()!="_Response") &&(normalizedKey.Trim()!="_Response2") && (normalizedKey.Trim()!="OptionalBytes_Index0_Response2") && (normalizedKey.Trim()!="OptionalBytes_Value0_Response2") && (!normalizedKey.StartsWith("SecurityLevel")) && (!item.Key.StartsWith("OptionalBytes_State_Request1")) && (!normalizedKey.StartsWith("Requesting")) && (!normalizedKey.StartsWith("SuppresssPositiveResponse")) && (!normalizedKey.StartsWith("SecurityLevel")) && (!item.Key.StartsWith("OptionalBytes_State_Request2")) && (!item.Key.StartsWith("OptionalBytes_Used")) && (normalizedKey.Trim() != "Responsing") && (normalizedKey.Trim() != "SecurityLevel") && (!item.Key.StartsWith("OptionalBytes_State_Response1")) && (!normalizedKey.StartsWith("Responsing")) && (!normalizedKey.StartsWith("SecurityLevel")) && (!item.Key.StartsWith("OptionalBytes_State_Response2")) && (item.Key.Trim() != "OptionalBytes_Index0_Request1") && (item.Key.Trim() != "OptionalBytes_Value0_Request1") && (item.Key.Trim() != "OptionalBytes_Index1_Response1") && (item.Key.Trim() != "OptionalBytes_Value1_Response1") && (item.Key.Trim() != "OptionalBytes_Index0_Request2") && (item.Key.Trim() != "OptionalBytes_Value0_Request2") && (item.Key.Trim() != "OptionalBytes_Index1_Response2") && (item.Key.Trim() != "OptionalBytes_Value1_Response2") && (item.Key.Trim() != ""))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }

                    }
                    //Regex regex = new Regex(@"(Request|Response)\d*");
                    foreach (var groupBoxName in groupBoxNames)
                    {


                        //TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == standardGroupName);


                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request1";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response1";
                        bool isRequestGroup1 = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request2";
                        bool isResponseGroup1 = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response2";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node =>
                                                                                     string.Equals(node.Header, groupBoxName, StringComparison.OrdinalIgnoreCase));


                        //var groupNumber = groupBoxName.Substring(groupBoxName.Length - 1); // Assumes single-digit numbering
                        string CleanGroupName(string name)
                        {
                            return System.Text.RegularExpressions.Regex.Replace(name, @"\d+$", "");
                        }
                        string cleanedGroupName1 = CleanGroupName(groupBoxName);

                        if (groupNode == null)
                        {
                            // Remove the numeric suffix from the group box name before adding to the tree view
                            //string cleanedGroupName = CleanGroupName(groupBoxName);

                            if (isRequestGroup || isResponseGroup || isRequestGroup1 || isResponseGroup1)
                            {
                              
                                // Create and add the new group node with the cleaned name for display
                                groupNode = new TreeNode { Header = cleanedGroupName1 };
                                selectedNode.Children.Add(groupNode);
                            }
                        }


                        if (groupNode != null)
                        {
                            // Define the order in which keys are processed for requests and responses
                            var requestKeysOrder = new List<string> { "Requesting", "SuppresssPositiveResponse", "SecurityLevel" }; // Add all relevant keys
                            var responseKeysOrder = new List<string> { "Responsing", "SecurityLevel" }; // Adjust according to your needs

                            // Temporarily store nodes to be added, preserving order
                            var tempNodes = new List<TreeNode>();

                            foreach (var keyOrder in (isRequestGroup || isRequestGroup1) ? requestKeysOrder : responseKeysOrder)
                            {
                                foreach (var item in collectedData)
                                {
                                    var (normalizedKey, numericSuffix) = NormalizeKeyName(item.Key);
                                    TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                    if (existingNode != null)
                                    {
                                        // Update the existing node's value
                                        existingNode.Header = $"{item.Key}: {item.Value}";
                                    }
                                    else
                                    {
                                        // Match the item with the current key order and group
                                        if (normalizedKey == keyOrder && ((isRequestGroup && numericSuffix == "1") || (isResponseGroup && numericSuffix == "1")))
                                        {
                                            // Create a new node for matching items
                                            TreeNode newNode = new TreeNode { Header = $"{normalizedKey}: {item.Value}" };
                                            groupNode.Children.Add(newNode);
                                        }
                                    }
                                    
                                }
                            }

                            // After ordering, add the nodes to the groupNode
                            //foreach (var node in tempNodes)
                            //{
                            //    groupNode.Children.Add(node);
                            //}
                        }


                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes_State_Request1"))
                                {
                                    string usedValue = collectedData["OptionalBytes_State_Request1"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}_Request1";
                                            string valueKey = $"OptionalBytes_Value{i}_Request1";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes_State_Response1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes_State_Response1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index{i}_Response1";
                                        string valueKey = $"OptionalBytes_Value{i}_Response1";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }


                        if (groupNode != null)
                        {
                            // Temporarily store nodes to be added, maintaining the order
                            var orderedNodes = new List<TreeNode>();
                            // Define the desired order for keys in Request and Response groups
                            List<string> keyOrder = new List<string> { "Requesting", "SuppresssPositiveResponse", "SecurityLevel", "Responsing" };

                            foreach (string key in keyOrder)
                            {
                                foreach (var item in collectedData)
                                {
                                    var (normalizedKey, numericSuffix) = NormalizeKeyName(item.Key);

                                    // Check if the current item matches the key we're processing
                                    if (normalizedKey == key && !item.Key.StartsWith("OptionalBytes"))
                                    {
                                        TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                        if (existingNode != null)
                                        {
                                            // Update the existing node's value
                                            existingNode.Header = $"{item.Key}: {item.Value}";
                                        }
                                        else
                                        {
                                            // Determine the group and numeric suffix condition
                                            bool shouldAdd = false;
                                            if (isRequestGroup1 && numericSuffix == "2")
                                            {
                                                shouldAdd = key == "Requesting" || key == "SuppresssPositiveResponse" || key == "SecurityLevel";
                                            }
                                            else if (isResponseGroup1 && numericSuffix == "2")
                                            {
                                                shouldAdd = key == "Responsing" || key == "SecurityLevel";
                                            }

                                            if (shouldAdd)
                                            {
                                                TreeNode newNode = new TreeNode { Header = $"{normalizedKey}: {item.Value}" };
                                                // Instead of directly adding, store in a temporary list to maintain order
                                                if (!orderedNodes.Any(n => n.Header == newNode.Header))
                                                {
                                                    orderedNodes.Add(newNode);
                                                }
                                            }
                                        }

                                    }
                                }
                            }

                            // Add nodes in the specified order
                            foreach (var node in orderedNodes)
                            {
                                groupNode.Children.Add(node);
                            }
                        }



                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup1 == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes_State_Request2"))
                                {
                                    string usedValue = collectedData["OptionalBytes_State_Request2"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}_Request2";
                                            string valueKey = $"OptionalBytes_Value{i}_Request2";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup1 == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes_State_Response2"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes_State_Response2"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index{0}_Response2";
                                        string valueKey = $"OptionalBytes_Value{0}_Response2";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }

                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh();
                    }
                }
            }
        }
        public void AddNewElementsToSelectedItem31(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
           

            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        var (normalizedKey, numericSuffix) = NormalizeKeyName(item.Key);
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!IsRequestItem(normalizedKey) && !IsUnnecessaryItem(normalizedKey))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && (item.Key== "RoutinControlType"|| item.Key== "RoutineControlName"||item.Key== "SuppressPossitiveResponse"||item.Key== "RoutineIdentifier"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                        
                                    }
                                    else if (isResponseGroup && (item.Key== "RoutinControlType" || item.Key== "SessionParameterRecord"))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }
                                }
                            }
                        }

                        // Handle OptionalBytes data if present

                        if (isRequestGroup == true)
                        {
                            TreeNode routineControlOptionRecordNode = groupNode.Children.FirstOrDefault ( node => node.Header == "RoutineControlOptionRecord" );
                            if (routineControlOptionRecordNode == null)
                            {
                                routineControlOptionRecordNode = new TreeNode { Header = "RoutineControlOptionRecord" };
                                groupNode.Children.Add(routineControlOptionRecordNode);
                            }
                            else
                            {
                                groupNode.Children.Remove(routineControlOptionRecordNode);
                               
                                groupNode.Children.Add(routineControlOptionRecordNode);
                            }
                            // Add Start Address and Size nodes under RoutineControlOptionRecord
                            //TreeNode routineControlOptionRecordNode = groupNode.Children.FirstOrDefault ( node => node.Header == "RoutineControlOptionRecord" );
                            TreeNode startAddressNode = routineControlOptionRecordNode.Children.FirstOrDefault(node =>node.Header == "Start Address" );
                            if(startAddressNode==null)
                            {
                                startAddressNode = new TreeNode { Header = "Start Address" };
                                routineControlOptionRecordNode.Children.Add(startAddressNode);
                            }
                            else
                            {
                                routineControlOptionRecordNode.Children.Clear();
                                routineControlOptionRecordNode.Children.Add(startAddressNode);
                            }
                            TreeNode sizeNode = routineControlOptionRecordNode.Children.FirstOrDefault(node => node.Header == "Size");
                            if(sizeNode==null)
                            {
                                 sizeNode = new TreeNode { Header = "Size" };
                                 routineControlOptionRecordNode.Children.Add(sizeNode);

                            }
                            else
                            {
                                routineControlOptionRecordNode.Children.Clear();
                                routineControlOptionRecordNode.Children.Add(sizeNode);
                            }

                            // Add values under Start Address and Size
                            string startAddressUsedValue = collectedData.ContainsKey("StartAddress_Used") ? collectedData["StartAddress_Used"] : "false";
                            string sizeUsedValue = collectedData.ContainsKey("Size_Used") ? collectedData["Size_Used"] : "false";



                            startAddressNode.Children.Add(new TreeNode { Header = $"Used: {startAddressUsedValue}" });
                            startAddressNode.Children.Add(new TreeNode { Header = $"Length: {(collectedData.ContainsKey("StartAddress_Value") ? collectedData["StartAddress_Value"] : "4")}" });


                            sizeNode.Children.Add(new TreeNode { Header = $"Used: {sizeUsedValue}" });
                            sizeNode.Children.Add(new TreeNode { Header = $"Length: {(collectedData.ContainsKey("Size_Value") ? collectedData["Size_Value"] : "4")}" });






                            if (collectedData.ContainsKey("CheksumStateRequest1") && collectedData["CheksumStateRequest1"] == "True")
                            {
                                TreeNode checkSumNode = new TreeNode { Header = "CheckSum" };
                                string ChekSumValueUsed = collectedData["Cheksum_Used"];
                                string ChekSumCrcValue = collectedData["CRC"];
                                checkSumNode.Children.Add(new TreeNode { Header = $"Used : {ChekSumValueUsed}" });
                                checkSumNode.Children.Add(new TreeNode { Header = $"CRC :{ChekSumCrcValue}" });
                                checkSumNode.Children.Add(new TreeNode { Header = $"Length : {(collectedData.ContainsKey("ChekSum_Value") ? collectedData["ChekSum_Value"] : "4")}" });
                                // Add the Request node to the selected node or another appropriate location
                                routineControlOptionRecordNode.Children.Add(checkSumNode);
                            }


                            // Check for Request OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes_State_Request1"))
                            {
                                // Create OptionalBytes node
                                string usedValueRequest = collectedData["OptionalBytes_Used"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeRequest = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                //routineControlOptionRecordNode.Children.Add(optionalBytesNodeRequest);
                                if (optionalBytesNodeRequest == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeRequest = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeRequest.Children.Add(new TreeNode { Header = usedValueRequest });
                                    //routineControlOptionRecordNode.Children.Add(optionalBytesNodeRequest);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeRequest.Children.Clear();
                                    optionalBytesNodeRequest.Children.Add(new TreeNode { Header = usedValueRequest });
                                    //routineControlOptionRecordNode.Children.Add(optionalBytesNodeRequest);
                                }

                                if (usedValueRequest == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeRequest = optionalBytesNodeRequest.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeRequest == null)
                                    {
                                        byteNodeRequest = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeRequest.Children.Add(byteNodeRequest);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Add index and value nodes under OptionalBytes
                                    for (int i = 0; i < int.MaxValue; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index{i}_Request1";
                                        string valueKey = $"OptionalBytes_Value{i}_Request1";

                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {
                                            TreeNode existingIndexNode = byteNodeRequest.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeRequest.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeRequest.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeRequest.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    
                                }
                                routineControlOptionRecordNode.Children.Add(optionalBytesNodeRequest);
                            }


                            if (collectedData.ContainsKey("OptionalBytes_State2_Request1"))
                            {
                                string usedValueRequest1 = collectedData["OptionalBytes_Used1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeRequest1 = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                
                                if (optionalBytesNodeRequest1 == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeRequest1 = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeRequest1.Children.Add(new TreeNode { Header = usedValueRequest1 });
                                    groupNode.Children.Add(optionalBytesNodeRequest1);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeRequest1.Children.Clear();
                                    optionalBytesNodeRequest1.Children.Add(new TreeNode { Header = usedValueRequest1 });
                                }

                                if (usedValueRequest1 == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeRequest1 = optionalBytesNodeRequest1.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeRequest1 == null)
                                    {
                                        byteNodeRequest1 = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeRequest1.Children.Add(byteNodeRequest1);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Add index and value nodes under OptionalBytes
                                    for (int i = 0; i < int.MaxValue; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index{i}_Request2";
                                        string valueKey = $"OptionalBytes_Value{i}_Request2";

                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {
                                            TreeNode existingIndexNode = byteNodeRequest1.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeRequest1.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeRequest1.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeRequest1.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }

                                
                            }
                        }

                        

                        if (isResponseGroup == true)
                        {
                            TreeNode routineInfoNode = groupNode.Children.FirstOrDefault(node => node.Header == "RoutineInfo");
                            if (routineInfoNode == null)
                            {
                                routineInfoNode = new TreeNode { Header = "RoutineInfo" };
                                groupNode.Children.Add(routineInfoNode);
                            }
                            else
                            {
                                groupNode.Children.Remove(routineInfoNode);
                                groupNode.Children.Add(routineInfoNode);
                            }
                            



                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes1");
                                //routineInfoNode.Children.Add(optionalBytesNodeResponse);
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });

                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Add index and value nodes under OptionalBytes
                                    for (int i = 0; i < int.MaxValue; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index{i}_Response1";
                                        string valueKey = $"OptionalBytes_Value{i}_Response1";

                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }


                                }
                                routineInfoNode.Children.Add(optionalBytesNodeResponse);


                            }




                            TreeNode routineStatusNode = groupNode.Children.FirstOrDefault(node => node.Header == "RoutineStatusRecord");
                            if (routineStatusNode == null)
                            {
                                routineStatusNode = new TreeNode { Header = "RoutineStatusRecord" };
                                groupNode.Children.Add(routineStatusNode);
                            }
                            else
                            {
                                groupNode.Children.Remove(routineStatusNode);
                                groupNode.Children.Add(routineStatusNode);
                            }

                            if (collectedData.ContainsKey("OptionalBytes2"))
                            {
                                string usedValueResponse1 = collectedData["OptionalBytes2"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse1 = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                
                                if (optionalBytesNodeResponse1 == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse1 = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse1.Children.Add(new TreeNode { Header = usedValueResponse1 });
                                    

                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse1.Children.Clear();
                                    optionalBytesNodeResponse1.Children.Add(new TreeNode { Header = usedValueResponse1 });
                                    
                                }

                                if (usedValueResponse1 == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse1 = optionalBytesNodeResponse1.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse1 == null)
                                    {
                                        byteNodeResponse1 = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse1.Children.Add(byteNodeResponse1);
                                       
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Add index and value nodes under OptionalBytes
                                    for (int i = 0; i < int.MaxValue; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index{i}_Response2";
                                        string valueKey = $"OptionalBytes_Value{i}_Response2";

                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {
                                            TreeNode existingIndexNode = byteNodeResponse1.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse1.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse1.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse1.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    
                                }
                                routineStatusNode.Children.Add(optionalBytesNodeResponse1);

                            }
                            

                            if (collectedData.ContainsKey("OptionalBytes3"))
                            {
                                string usedValueResponse2 = collectedData["OptionalBytes3"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse2 = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                //groupNode.Children.Add(optionalBytesNodeResponse);
                                if (optionalBytesNodeResponse2 == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse2 = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse2.Children.Add(new TreeNode { Header = usedValueResponse2});
                                    groupNode.Children.Add(optionalBytesNodeResponse2);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse2.Children.Clear();
                                    optionalBytesNodeResponse2.Children.Add(new TreeNode { Header = usedValueResponse2 });
                                    
                                }

                                if (usedValueResponse2 == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse2 = optionalBytesNodeResponse2.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse2 == null)
                                    {
                                        byteNodeResponse2 = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse2.Children.Add(byteNodeResponse2);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Add index and value nodes under OptionalBytes
                                    for (int i = 0; i < int.MaxValue; i++) 
                                    {
                                        string indexKey = $"OptionalBytes_Index{i}_Response3";
                                        string valueKey = $"OptionalBytes_Value{i}_Response3";

                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {
                                            TreeNode existingIndexNode = byteNodeResponse2.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse2.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse2.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse2.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }


                                }

                                

                            }
                        }


                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh(); // This line might not be necessary
                        //this.Close();
                    }
                }
            }
        }


        // Helper method to check if a key is relevant to Request
        private bool IsRequestItem(string key)
        {
            return key == "RoutineControlType" || key == "RoutineControlName" || key == "SuppressPositiveResponse" || key == "RoutineIdentifier";
        }

        // Helper method to check if a key is unnecessary
        private bool IsUnnecessaryItem(string key)
        {
            return key == "RoutineControlOptionRecord_State_Request" || key == "StartAddress_Used" || key == "StartAddress_Value" || key == "Size_Used" || key == "Size_Value" || key.StartsWith("OptionalBytes_State_Request1") || key.StartsWith("RoutinControlType")   ||key.StartsWith("RoutineStatusRecord_OptionalBytes_Index") || key.StartsWith("_Request") || key.StartsWith("_Response") || key.StartsWith("OptionalBytes_Used") || key.StartsWith("RoutineInfo_State") || key.StartsWith("RoutineStatusRecord_State") || key.StartsWith("OptionalBytes_State") || key.StartsWith("OptionalBytes_OptionalBytes") || key.StartsWith("RoutineStatusRecord_OptionalBytes_Value") || key.StartsWith("RoutineInfo_OptionalBytes") || key.StartsWith("RoutineInfo_OptionalBytes_Index") | key.StartsWith("RoutineInfo_OptionalBytes_Value") || key.StartsWith("RoutineStatusRecord") || key.StartsWith("RoutineStatusRecord_OptionalBytes") || key.StartsWith("OptionalBytes_Index") || key.StartsWith("OptionalBytes_Value") || key.StartsWith("RoutineConrolType") || key.StartsWith("SessionParameterRecord") || key.StartsWith("RoutineInfo")|| key.StartsWith("OptionalBytes")|| key.StartsWith("CheksumStateRequest1")|| key.StartsWith("Cheksum_Used")||key.StartsWith("CRC")|| key.StartsWith("ChekSum_Value")||key.StartsWith("CheksumStateRequest")|| key.StartsWith("SupressPossitiveResponse");
        }
        private bool IsUnnecessaryRespItem(string key)
        {
            return key == "RoutineControlOptionRecord_State_Request" || key == "StartAddress_Used" || key == "StartAddress_Value" || key == "Size_Used" || key == "Size_Value" || key.StartsWith("OptionalBytes_State_Request1") || key.StartsWith("RoutineStatusRecord_OptionalBytes_Index") || key.StartsWith("_Request") || key.StartsWith("OptionalBytes_Used") || key.StartsWith("RoutineInfo_State") || key.StartsWith("RoutineStatusRecord_State") || key.StartsWith("OptionalBytes_State") || key.StartsWith("OptionalBytes_OptionalBytes") || key.StartsWith("RoutineStatusRecord_OptionalBytes_Value") || key.StartsWith("RoutineInfo_OptionalBytes") || key.StartsWith("RoutineInfo_OptionalBytes_Index") | key.StartsWith("RoutineInfo_OptionalBytes_Value") || key.StartsWith("RoutineStatusRecord") || key.StartsWith("RoutineStatusRecord_OptionalBytes") || key.StartsWith("OptionalBytes_Index") || key.StartsWith("OptionalBytes_Value") || key.StartsWith("RoutineInfo")||key.StartsWith("OptionalBytes");
        }
        // Helper method to check if a key is relevant to Response
        private bool IsResponseItem(string key)
        {
            return key == "RoutineControlType" || key == "SessionParameterRecord" ;
        }


        public void AddNewElementsToSelectedItem85(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!item.Key.StartsWith("SessionType") && !item.Key.StartsWith("SessionName") && !item.Key.StartsWith("SuppressPositiveResponse") )
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && (item.Key == "SessionType" || item.Key == "SessionName" || item.Key == "SuppressPositiveResponse"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (isResponseGroup && (item.Key == "SessionType1" || item.Key == "SessionParameterRecord"))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }
                                    
                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes"))
                                {
                                    string usedValue = collectedData["OptionalBytes"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}";
                                            string valueKey = $"OptionalBytes_Value{i}";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index1{i}";
                                        string valueKey = $"OptionalBytes_Value1{i}";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }


                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh();
                    }
                }
            }
        }

        public void AddNewElementsToSelectedItem2E(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!item.Key.StartsWith("OptionalBytes") && !item.Key.StartsWith("DIDNmae") && !item.Key.StartsWith("SuppressPositiveResponse") && !item.Key.StartsWith("DataIdentifier") && !item.Key.StartsWith("Write_Data_Bytes") && !item.Key.StartsWith("Length") && !item.Key.StartsWith("SessionParameterRecord"))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && (item.Key == "DIDNmae" || item.Key == "SuppressPositiveResponse" || item.Key == "DataIdentifier" || item.Key == "Length" || item.Key == "Write_Data_Bytes"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (isResponseGroup && (item.Key == "SessionParameterRecord" ))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }
                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes"))
                                {
                                    string usedValue = collectedData["OptionalBytes"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}_Request1";
                                            string valueKey = $"OptionalBytes_Value{i}_Request1";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        //string indexKey = $"OptionalBytes_Index1{i}";
                                        //string valueKey = $"OptionalBytes_Value1{i}";
                                        string indexKey = $"OptionalBytes_Index{i} _Response1";
                                        string valueKey = $"OptionalBytes_Value{i} _Response1";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }


                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh();
                    }
                }
            }
        }

        public void AddNewElementsToSelectedItem22(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {

            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!item.Key.StartsWith("OptionalBytes") && !item.Key.StartsWith("DIDNmae") && !item.Key.StartsWith("SuppressPositiveResponse") && !item.Key.StartsWith("DataIdentifier") && !item.Key.StartsWith("Write_Data_Bytes") && !item.Key.StartsWith("Length") && !item.Key.StartsWith("SessionParameterRecord"))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && (item.Key == "DIDNmae" || item.Key == "SuppressPositiveResponse" || item.Key == "DataIdentifier" || item.Key == "Length" || item.Key == "Write_Data_Bytes"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (isResponseGroup && (item.Key == "SessionParameterRecord"))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }
                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes"))
                                {
                                    string usedValue = collectedData["OptionalBytes"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}_Request1";
                                            string valueKey = $"OptionalBytes_Value{i}_Request1";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        //string indexKey = $"OptionalBytes_Index1{i}";
                                        //string valueKey = $"OptionalBytes_Value1{i}";
                                        string indexKey = $"OptionalBytes_Index{i} _Response1";
                                        string valueKey = $"OptionalBytes_Value{i} _Response1";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }


                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh();
                    }
                }
            }
        }

        public void AddNewElementsToSelectedItem34(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
           
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        var (normalizedKey, numericSuffix) = NormalizeKeyName(item.Key);
                        //TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                       // Check if it's not a request item and it's not unnecessary
                        if (!IsRequestItem1(normalizedKey) && !IsUnnecessaryItem1(normalizedKey) && !IsUnnecessaryRespItem1(normalizedKey))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    var (normalizedKey, numericSuffix) = NormalizeKeyName(item.Key);
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (IsRequestItem1(normalizedKey) && !!IsUnnecessaryItem1(normalizedKey))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (IsResponseItem1(normalizedKey) && !IsUnnecessaryRespItem1(normalizedKey))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }
                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                //Create RoutineControlOptionRecord node
                                TreeNode MemoryAdressNode = new TreeNode { Header = "MemoryAddress" };
                                if(MemoryAdressNode==null)
                                {
                                    groupNode.Children.Add(MemoryAdressNode);
                                }
                                else
                                {
                                    groupNode.Children.Clear();
                                    groupNode.Children.Add(MemoryAdressNode);
                                }
                               

                                // Add Start Address and Size nodes under RoutineControlOptionRecord
                                TreeNode lengthLabelNode = new TreeNode { Header = $"Length : {collectedData["Length1"]}" };


                                //lengthLabelNode.Children.Add(sizeLengthValueNode);
                                MemoryAdressNode.Children.Add(lengthLabelNode);

                                TreeNode MemorySizeNode = new TreeNode { Header = "MemorySize" };
                                groupNode.Children.Add(MemorySizeNode);

                                TreeNode lengthLabelNodeSize = new TreeNode { Header = $"Length : {collectedData["Length2"]}" };

                                //TreeNode valueOfLengthNode = new TreeNode {Header = collectedData["Length2"] };
                                //lengthLabelNodeSize.Children.Add(valueOfLengthNode);
                                MemorySizeNode.Children.Add(lengthLabelNodeSize);
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes"))
                                {
                                    string usedValue = collectedData["OptionalBytes"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}_Request1";
                                            string valueKey = $"OptionalBytes_Value{i}_Request1";
                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            TreeNode MaxNumberOfBlockLengthNode = new TreeNode { Header = "MaxNumberOfBlockLength" };
                            if(MaxNumberOfBlockLengthNode==null)
                            {
                                groupNode.Children.Add(MaxNumberOfBlockLengthNode);
                            }
                            else
                            {
                                groupNode.Children.Clear();
                                groupNode.Children.Add(MaxNumberOfBlockLengthNode);
                            }

                            TreeNode LengthNode = new TreeNode { Header = $"Length : {collectedData["Length3"]}" };


                            //LengthNode.Children.Add(SizeoflengthNode);

                            MaxNumberOfBlockLengthNode.Children.Add(LengthNode);
                            //Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index{i} _Response1";
                                        string valueKey = $"OptionalBytes_Value{i} _Response1";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }
                        // Filter out nodes with the header "Service-10_Diagnostic"
                        

                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh(); // This line might not be necessary

                    }
                }
            }
        }
        private bool IsRequestItem1(string key)
        {
            return key == "DataFormatIdentifier" || key == "AddressAndLengthFormatIdentifier" || key == "SupressPossitiveResponse";
        }

        // Helper method to check if a key is unnecessary
        private bool IsUnnecessaryItem1(string key)
        {
            return key == "OptionalBytes" || key == "LengthFormatIdentifier" || key == "MaxNumberOfBlockLength" || key == "SuppressPositiveResponse" || key == "MemoryAddress" || key.StartsWith("MemorySize") || key.StartsWith("Length") || key.StartsWith("_Request") || key.StartsWith("OptionalBytes_Used") || key.StartsWith("RoutineInfo_State") || key.StartsWith("RoutineStatusRecord_State") || key.StartsWith("OptionalBytes_State") || key.StartsWith("OptionalBytes_OptionalBytes") || key.StartsWith("RoutineStatusRecord_OptionalBytes_Value") || key.StartsWith("_Response");
        }
        private bool IsUnnecessaryRespItem1(string key)
        {
            return key == "RoutineControlOptionRecord_State_Request" || key == "StartAddress_Used" || key == "StartAddress_Value" || key == "Size_Used" || key == "Size_Value" || key.StartsWith("OptionalBytes_State_Request1") || key.StartsWith("RoutineStatusRecord_OptionalBytes_Index") || key.StartsWith(" ") || key.StartsWith("OptionalBytes_Used") || key.StartsWith("RoutineInfo_State") || key.StartsWith("RoutineStatusRecord_State") || key.StartsWith("OptionalBytes_State") || key.StartsWith("OptionalBytes_OptionalBytes") || key.StartsWith("RoutineStatusRecord_OptionalBytes_Value") || key.StartsWith("RoutineInfo_OptionalBytes") || key.StartsWith("RoutineInfo_OptionalBytes_Index") | key.StartsWith("RoutineInfo_OptionalBytes_Value") || key.StartsWith("RoutineStatusRecord") || key.StartsWith("RoutineStatusRecord_OptionalBytes") || key.StartsWith("OptionalBytes_Index") || key.StartsWith("OptionalBytes_Value") || key.StartsWith("RoutineInfo")|| key.StartsWith("_Response");
        }
        // Helper method to check if a key is relevant to Response
        private bool IsResponseItem1(string key)
        {
            return key == "LengthFormatIdentifier";
        }
        public void AddNewElementsToSelectedItem36(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!item.Key.StartsWith("OptionalBytes") && !item.Key.StartsWith("BlockSequenceCounter") && !item.Key.StartsWith("OptionalBytes1") && !item.Key.StartsWith("SuppressPositiveResponse"))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && (item.Key == "BlockSequenceCounter" || item.Key == "SuppressPositiveResponse"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (isResponseGroup &&
                                             (item.Key != "SID" &&
                                              item.Key != "Service_Name" &&
                                              item.Key != "Service_Process_Name" &&
                                              item.Key != "Service_Request_Type" &&
                                              item.Key != "ServiceGapTimeout" &&
                                              item.Key != "BlockSequenceCounter" &&
                                              item.Key != "SuppressPositiveResponse"))

                                    {
                                        groupNode.Children.Add(newNode);
                                    }

                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes"))
                                {
                                    string usedValue = collectedData["OptionalBytes"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}_Request1";
                                            string valueKey = $"OptionalBytes_Value{i}_Request1";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index{i}_Response1";
                                        string valueKey = $"OptionalBytes_Value{i}_Response1";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }


                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh();
                    }
                }
            }
        }

        public void AddNewElementsToSelectedItem37(Dictionary<string, string> collectedData, List<string> groupBoxNames)
        {
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!item.Key.StartsWith("OptionalBytes") &&  !item.Key.StartsWith("SuppressPositiveResponse"))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && ( item.Key == "SuppressPositiveResponse"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (isResponseGroup &&
                                      (item.Key != "SID" &&
                                       item.Key != "Service_Name" &&
                                       item.Key != "Service_Process_Name" &&
                                       item.Key != "Service_Request_Type" &&
                                       item.Key != "ServiceGapTimeout" &&
                                       item.Key != "SuppressPositiveResponse"))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }

                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes"))
                                {
                                    string usedValue = collectedData["OptionalBytes"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        optionalBytesNode.Children.Clear();
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}_Request1";
                                            string valueKey = $"OptionalBytes_Value{i}_Request1";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index{i}_Response1";
                                        string valueKey = $"OptionalBytes_Value{i}_Response1";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }


                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh();
                    }
                }
            }
        }

        public void UpdateTreeViewWithCollectedData(Dictionary<string, string> collectedData, List<String> groupBoxNames)
        {
            if (_selectedTreeViewItem != null && _selectedTreeViewItem.DataContext != null)
            {
                TreeNode selectedNode = _selectedTreeViewItem.DataContext as TreeNode;
                if (selectedNode != null)
                {
                    // First, add non-request textboxes directly under the selected node
                    foreach (var item in collectedData)
                    {
                        // Adjust the condition to include all relevant keys for non-request textboxes
                        if (!item.Key.StartsWith("OptionalBytes") && !item.Key.StartsWith("ResetType") && !item.Key.StartsWith("ResetTypeName") && !item.Key.StartsWith("SuppressPositiveResponse") && !item.Key.StartsWith("SessionParameterRecord"))
                        {
                            // Check if a node with the same key already exists
                            TreeNode existingNode = selectedNode.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                            if (existingNode != null)
                            {
                                // Update the existing node's value
                                existingNode.Header = $"{item.Key}: {item.Value}";
                            }
                            else
                            {
                                // Handle non-OptionalBytes data
                                TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                // Add other items directly under the selected node
                                selectedNode.Children.Add(newNode);
                            }
                        }
                    }

                    foreach (var groupBoxName in groupBoxNames)
                    {
                        bool isRequestGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Request";
                        bool isResponseGroup = !string.IsNullOrWhiteSpace(groupBoxName) && groupBoxName == "Response";
                        TreeNode groupNode = selectedNode.Children.FirstOrDefault(node => node.Header == groupBoxName);
                        if (groupNode == null)
                        {
                            // Handle Request and Response group node creation if applicable
                            if (isRequestGroup || isResponseGroup)
                            {
                                groupNode = new TreeNode { Header = groupBoxName };
                                selectedNode.Children.Add(groupNode);
                            }
                        }

                        // Process collected data for Request and Response nodes
                        foreach (var item in collectedData)
                        {
                            if (!item.Key.StartsWith("OptionalBytes"))
                            {
                                // Check if a node with the same key already exists in the groupNode
                                TreeNode existingNode = groupNode?.Children.FirstOrDefault(node => node.Header.StartsWith($"{item.Key}:"));
                                if (existingNode != null)
                                {
                                    // Update the existing node's value
                                    existingNode.Header = $"{item.Key}: {item.Value}";
                                }
                                else
                                {
                                    // Handle non-OptionalBytes data
                                    TreeNode newNode = new TreeNode { Header = $"{item.Key}: {item.Value}" };
                                    if (isRequestGroup && (item.Key == "ResetType" || item.Key == "ResetTypeName" || item.Key == "SuppressPositiveResponse"))
                                    {
                                        // Add specific items under Request or Response node
                                        groupNode.Children.Add(newNode);
                                    }
                                    else if (isResponseGroup && (item.Key == "ResetType1" || item.Key == "PowerDownTime"))
                                    {
                                        groupNode.Children.Add(newNode);
                                    }
                                }
                            }
                        }

                        // Handle OptionalBytes data if present
                        if (collectedData.Keys.Any(k => k.StartsWith("OptionalBytes")))
                        {
                            if (isRequestGroup == true)
                            {
                                // Check for Request OptionalBytes
                                if (collectedData.ContainsKey("OptionalBytes"))
                                {
                                    string usedValue = collectedData["OptionalBytes"] == "True" ? "Used: True" : "Used: False";
                                    TreeNode optionalBytesNode = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                    if (optionalBytesNode == null)
                                    {
                                        // Create a new OptionalBytes node if it doesn't exist
                                        optionalBytesNode = new TreeNode { Header = "OptionalBytes" };
                                        optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                        groupNode.Children.Add(optionalBytesNode);
                                    }
                                    else
                                    {
                                        //optionalBytesNode.Header = $"{item.Key}: {item.Value}";
                                        // Update the existing OptionalBytes node's value
                                        // optionalBytesNode.Children.Clear();
                                        //optionalBytesNode.Children.Add(new TreeNode { Header = usedValue });
                                    }

                                    if (usedValue == "Used: True")
                                    {
                                        // Add "Index" and "Value" under Byte node if present for Request
                                        TreeNode byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                        if (byteNode == null)
                                        {
                                            byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                            optionalBytesNode.Children.Add(byteNode);
                                        }

                                        // Determine the maximum count of either indices or values
                                        int maxCount = Math.Max(
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                            collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                        );

                                        if (byteNode != null)
                                        {
                                            optionalBytesNode.Children.Remove(byteNode);
                                            byteNode = optionalBytesNode.Children.FirstOrDefault(node => node.Header == "Byte");
                                            if (byteNode == null)
                                            {
                                                byteNode = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                                optionalBytesNode.Children.Add(byteNode);
                                            }

                                        }

                                        // Loop over the keys and add each value to the TreeNode
                                        for (int i = 0; i < maxCount; i++)
                                        {
                                            string indexKey = $"OptionalBytes_Index{i}";
                                            string valueKey = $"OptionalBytes_Value{i}";

                                            // Check if both the index and value keys exist
                                            if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                            {
                                                if (collectedData[indexKey] == "" && collectedData[valueKey] == "")
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Find the existing index and value nodes
                                                    TreeNode existingIndexNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                                    TreeNode existingValueNode = byteNode.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                                    if (existingIndexNode != null)
                                                    {
                                                        // Update the existing index node's value
                                                        existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the index to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                                    }

                                                    if (existingValueNode != null)
                                                    {
                                                        // Update the existing value node's value
                                                        existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                                    }
                                                    else
                                                    {
                                                        // Add the value to the TreeNode if it doesn't exist
                                                        byteNode.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Optionally, handle the case where an index or value is missing
                                                // For example, you might want to add a placeholder or skip this iteration
                                            }
                                        }


                                    }
                                }
                            }
                        }
                        if (isResponseGroup == true)
                        {
                            // Check for Response OptionalBytes
                            if (collectedData.ContainsKey("OptionalBytes1"))
                            {
                                string usedValueResponse = collectedData["OptionalBytes1"] == "True" ? "Used: True" : "Used: False";
                                TreeNode optionalBytesNodeResponse = groupNode.Children.FirstOrDefault(node => node.Header == "OptionalBytes");
                                if (optionalBytesNodeResponse == null)
                                {
                                    // Create a new OptionalBytes node if it doesn't exist
                                    optionalBytesNodeResponse = new TreeNode { Header = "OptionalBytes" };
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                    groupNode.Children.Add(optionalBytesNodeResponse);
                                }
                                else
                                {
                                    // Update the existing OptionalBytes node's value
                                    optionalBytesNodeResponse.Children.Clear();
                                    optionalBytesNodeResponse.Children.Add(new TreeNode { Header = usedValueResponse });
                                }

                                if (usedValueResponse == "Used: True")
                                {
                                    // Add "Index" and "Value" under Byte node if present for Response
                                    TreeNode byteNodeResponse = optionalBytesNodeResponse.Children.FirstOrDefault(node => node.Header == "Byte");
                                    if (byteNodeResponse == null)
                                    {
                                        byteNodeResponse = new TreeNode { Header = "Byte" }; // Add Byte node under OptionalBytes
                                        optionalBytesNodeResponse.Children.Add(byteNodeResponse);
                                    }

                                    int maxCount = Math.Max(
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Index")),
                                        collectedData.Keys.Count(key => key.StartsWith("OptionalBytes_Value"))
                                    );

                                    // Loop over the keys and add each value to the TreeNode
                                    for (int i = 0; i < maxCount; i++)
                                    {
                                        string indexKey = $"OptionalBytes_Index1{i}";
                                        string valueKey = $"OptionalBytes_Value1{i}";

                                        // Check if both the index and value keys exist
                                        if (collectedData.ContainsKey(indexKey) && collectedData.ContainsKey(valueKey))
                                        {

                                            // Find the existing index and value nodes for the Response group
                                            TreeNode existingIndexNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Index: {collectedData[indexKey]}"));
                                            TreeNode existingValueNode = byteNodeResponse.Children.FirstOrDefault(node => node.Header.StartsWith($"Value: {collectedData[valueKey]}"));

                                            if (existingIndexNode != null)
                                            {
                                                // Update the existing index node's value for the Response group
                                                existingIndexNode.Header = $"Index: {collectedData[indexKey]}";
                                            }
                                            else
                                            {
                                                // Add the index to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Index: {collectedData[indexKey]}" });
                                            }

                                            if (existingValueNode != null)
                                            {
                                                // Update the existing value node's value for the Response group
                                                existingValueNode.Header = $"Value: {collectedData[valueKey]}";
                                            }
                                            else
                                            {
                                                // Add the value to the TreeNode for the Response group if it doesn't exist
                                                byteNodeResponse.Children.Add(new TreeNode { Header = $"Value: {collectedData[valueKey]}" });
                                            }
                                        }
                                        else
                                        {
                                            // Optionally, handle the case where an index or value is missing
                                            // For example, you might want to add a placeholder or skip this iteration
                                        }
                                    }
                                }
                            }
                        }



                        // Refresh the TreeView to reflect the changes
                        treeView.Items.Refresh();
                    }
                }
            }
        }

        private void UpdateTreeView(Dictionary<string, string> data)
        {

            treeView.Items.Refresh();
            Debug.WriteLine($"Updated TreeView. SelectedNode: {_selectedTreeViewItem?.DataContext}");
        }

        #endregion


        static T VisualUpwardSearch<T>(DependencyObject source) where T : DependencyObject
        {
            while (source != null && !(source is T))
                source = VisualTreeHelper.GetParent(source);

            return source as T;
        }



        private void UpdateOrAddTemplateNodeToRoot(string label, string value)
        {
            // Ensure the TreeNodes collection is initialized
            if (TreeNodes == null)
            {
                TreeNodes = new ObservableCollection<TreeNode>();
            }

            // Ensure the TreeNodes collection and the root node exist
            if (TreeNodes.Count == 0)
            {
                TreeNodes.Add(new TreeNode { Header = "TemplateInfo" });
            }


            // Assuming the root node is always the first item
            var rootNode = TreeNodes[0];

            // Find or create the target node
            var targetNode = rootNode.Children.FirstOrDefault(n => n.Header.StartsWith(label + ":"));
            if (targetNode == null)
            {
                targetNode = new TreeNode { Header = $"{label}: {value}" };
                rootNode.Children.Add(targetNode);
            }
            else
            {
                targetNode.Header = $"{label}: {value}";
            }

            // Refresh the TreeView to reflect changes
            treeView.Items.Refresh();
        }

        private void BaudRateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BaudRateComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        //private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        //{
        //    // Create OpenFileDialog
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Filter = "XML files (*.xml)|*.xml|sTem files (*.sTemp)|*.sTemp|All files (*.*)|*.*";


        //    // Show the dialog and process the selected file
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        try
        //        {
        //            // Read the contents of the selected XML file
        //            string xmlContent = File.ReadAllText(openFileDialog.FileName);

        //            // Display the XML content in the RichTextBox
        //            XmlRichTextBox.Document.Blocks.Clear();
        //            XmlRichTextBox.Document.Blocks.Add(new Paragraph(new Run(xmlContent)));


        //            // Load XML content directly into your TreeView structure
        //            LoadXmlIntoTreeViewAndUI(xmlContent);
        //            LoadXmlIntoTreeView(xmlContent);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error loading XML file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            string lastSavedFilePath = Properties.Settings.Default.LastSavedFilePath;

            if (!string.IsNullOrEmpty(lastSavedFilePath) && File.Exists(lastSavedFilePath))
            {
                try
                {
                    string xmlContent = File.ReadAllText(lastSavedFilePath);
                    XmlRichTextBox.Document.Blocks.Clear();
                    XmlRichTextBox.Document.Blocks.Add(new Paragraph(new Run(xmlContent)));

                    LoadXmlIntoTreeViewAndUI(xmlContent);
                    LoadXmlIntoTreeView(xmlContent);

                    MessageBox.Show($"XML successfully loaded from {lastSavedFilePath}", "Load Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading XML file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No file has been saved or file does not exist.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadXmlIntoTreeView(string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent); // Use Parse instead of Load
            TreeNodes.Clear();
            LoadNode(doc.Root, null);
            
        }


        private void LoadNode(XElement xmlElement, TreeNode parentNode)
        {
            // Start with the element's name
            string header = xmlElement.Name.LocalName;

            // Append attributes directly to the header
            foreach (var attribute in xmlElement.Attributes())
            {
                header += $" {attribute.Name}: {attribute.Value}";
            }

            // Append value directly to the header if there are no child elements
            if (!xmlElement.HasElements && !string.IsNullOrWhiteSpace(xmlElement.Value))
            {
                header += $": {xmlElement.Value.Trim()}";
            }

            // Special handling for OEM with TemplateName
            if (header.StartsWith("OEM") && xmlElement.Elements().Any(x => x.Name.LocalName == "TemplateName"))
            {
                header += $" TemplateName: {xmlElement.Elements().First(x => x.Name.LocalName == "TemplateName").Value.Trim()}";
            }

            // Check if the current node's header matches the last selected service type
            bool isLastSelectedService = header.Contains(lastSelectedServiceType);

            // Create the tree node with the updated header
            TreeNode newNode = new TreeNode(header)
            {
                // Optionally highlight or differentiate nodes that match the last selected service type
                IsHighlighted = isLastSelectedService // Assuming TreeNode has an IsHighlighted property for UI purposes
            };

            if (parentNode == null)
            {
                TreeNodes.Add(newNode);
            }
            else
            {
                parentNode.Children.Add(newNode);
            }

            // Recursively process child elements, skipping TemplateName if under OEM
            foreach (XElement childElement in xmlElement.Elements())
            {
                if (!(childElement.Name.LocalName == "TemplateName" && xmlElement.Name.LocalName == "OEM"))
                {
                    LoadNode(childElement, newNode);
                }
            }
        }



        private void LoadXmlIntoTreeViewAndUI(string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            TreeNodes.Clear();
            UpdateUIFromXml(doc.Root, null);
        }

        private void UpdateUIFromXml(XElement xmlElement, TreeNode parentNode)
        {
            TreeNode newNode = new TreeNode(xmlElement.Name.LocalName);

            // Append attributes and values to the node header appropriately
            if (!xmlElement.HasElements && xmlElement.Attributes().Count() == 0)
            {
                newNode.Header = xmlElement.Value.Trim();
            }
            else
            {
                foreach (var attribute in xmlElement.Attributes())
                {
                    newNode.Header += $" {attribute.Name}: {attribute.Value}";
                }
            }

            // Update textboxes if the elements are TemplateName or OEM
            if (xmlElement.Name.LocalName == "TemplateName")
            {
                Dispatcher.Invoke(() => TemplateNameTextBox.Text = xmlElement.Value.Trim());
            }
            else if (xmlElement.Name.LocalName == "OEM")
            {
                Dispatcher.Invoke(() => OEMTextBox.Text = xmlElement.Value.Trim());
            }
            else if (xmlElement.Name.LocalName == "Types")
            {
                Dispatcher.Invoke(() => TypesTextBox.Text = xmlElement.Value.Trim());
                Dispatcher.Invoke(() => TypesComboBox.Text = xmlElement.Value.Trim());
            }
            else if (xmlElement.Name.LocalName == "CommunicationProtocol")
            {
                Dispatcher.Invoke(() => CProtocolTextbox.Text = xmlElement.Value.Trim());
            }
            else if (xmlElement.Name.LocalName == "CommunicationDevice")
            {
                Dispatcher.Invoke(() => CDeviceTextbox.Text = xmlElement.Value.Trim());
            }
            else if (xmlElement.Name.LocalName == "BaudRate")
            {
                Dispatcher.Invoke(() => BaudRateComboBox.Text = xmlElement.Value.Trim());
            }
            else if(xmlElement.Name.LocalName== "PhysicalCANID")
            {
                Dispatcher.Invoke(() => physicalTextbox.Text = xmlElement.Value.Trim());
            }
            else if (xmlElement.Name.LocalName == "ResponseCANID")
            {
                Dispatcher.Invoke(() => ResponseTextbox.Text = xmlElement.Value.Trim());
            }
            else if (xmlElement.Name.LocalName == "FunctionalCANID")
            {
                Dispatcher.Invoke(() => FuntionalTextbox.Text = xmlElement.Value.Trim());
            }



            // Add the node to the TreeView
            if (parentNode == null)
            {
                TreeNodes.Add(newNode);
            }
            else
            {
                parentNode.Children.Add(newNode);
            }

            // Recursively process child elements
            foreach (XElement child in xmlElement.Elements())
            {
                UpdateUIFromXml(child, newNode);
            }
        }











    }

}




