using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Windows.Forms;

namespace BMICalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [XmlRoot("BMICalculator", Namespace = "www.bmicalc.ninja")]
        
        public class Customer
        {
            [XmlAttribute("Last Name")]
            public string lastName { get; set; }
            [XmlAttribute("First Name")]
            public string firstName { get; set; }
            [XmlAttribute("Phone Number")]
            public string phoneNumber { get; set; }
            [XmlAttribute("Height")]
            public int heightInches { get; set; }
            [XmlAttribute("Weight")]
            public int weightLbs { get; set; }
            [XmlAttribute("Customer BMI")]
            public double custBMI { get; set; }
            [XmlAttribute("Status")]
            public string statusTitle { get; set; }

            public string FilePath = System.IO.Path.GetDirectoryName("yourBMI.xml");
            public string FileName = "yourBMI.xml";

            
        }

        double userBMI;
        string phoneNumberWithDashes;
        int numOfXmlsInUse;
        int countDownDaXmls;
        string xmlNamingConvention = "yourBMI";
        string xmlEnding = ".xml";
        string tempXmlFilePath;
        int whichTableBox = 0;

        public MainWindow()
        {
            InitializeComponent();

            OnLoadStats();
            
        }
        #region Part1 Code
        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            xFirstNameBox.Text = "";
            xLastNameBox.Text = "";
            xPhoneBox.Text = "";
            xHeightInchesBox.Text = "";
            xWeightLbsBox.Text = "";
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            Customer customer1 = new Customer();
            #region Part 2 Code

            //double userBMI = (Convert.ToInt32(xWeightLbsBox.Text) / (Convert.ToInt32(xHeightInchesBox.Text) * Convert.ToInt32(xHeightInchesBox.Text)) * 703);

            customer1.lastName = Convert.ToString(xLastNameBox.Text);
            customer1.firstName = Convert.ToString(xFirstNameBox.Text);
            customer1.phoneNumber = Convert.ToString(xPhoneBox.Text);
            customer1.heightInches = Convert.ToInt32(xHeightInchesBox.Text);
            customer1.weightLbs = Convert.ToInt32(xWeightLbsBox.Text);
            userBMI = (customer1.weightLbs * 703) / Math.Pow(customer1.heightInches, 2);
            customer1.custBMI = userBMI;
            string yourBMIStatus = "NA";
            customer1.statusTitle = yourBMIStatus;

            int heightFeet = Convert.ToInt32(Math.Floor(Convert.ToDouble(customer1.heightInches / 12)));
            int heightInchesRemaining = Convert.ToInt32(customer1.heightInches % 12);

            var aStringBuilder = new StringBuilder(customer1.phoneNumber);      
            aStringBuilder.Insert(3, "-");
            aStringBuilder.Insert(7, "-");
            phoneNumberWithDashes = aStringBuilder.ToString();

            if (heightInchesRemaining == 0)
            {
                xSubmitPopupTextBlockOne.Text = ($"The customer's name is {customer1.firstName} {customer1.lastName} and they can be reached at {phoneNumberWithDashes}. They are {heightFeet} feet tall. Their weight is {customer1.weightLbs}. Their BMI is {Convert.ToString(Math.Floor(customer1.custBMI * 100) / 100)}.");
            }
            else
            {
                xSubmitPopupTextBlockOne.Text = ($"The customer's name is {customer1.firstName} {customer1.lastName} and they can be reached at {phoneNumberWithDashes}. They are {heightFeet} feet and {heightInchesRemaining} inches tall. Their weight is {customer1.weightLbs}. Their BMI is {Convert.ToString(Math.Floor(customer1.custBMI * 100) / 100)}.");
            }

            xYourBMIResults.Text = Convert.ToString(Math.Floor(customer1.custBMI * 100) / 100);

            if(customer1.custBMI < 18.5)
            {
                xBMIMessage.Text = "According to CDC.gov BMI Calculator you are underweight.";
                customer1.statusTitle = "Underweight";
            }
            else if (customer1.custBMI < 24.9)
            {
                xBMIMessage.Text = "According to CDC.gov BMI Calculator you have a normal body weight.";
                customer1.statusTitle = "Normal";
            }
            else if (customer1.custBMI < 29.9)
            {
                xBMIMessage.Text = "According to CDC.gov BMI Calculator you are overweight.";
                customer1.statusTitle = "Overweight";
            }
            else
            {
                xBMIMessage.Text = "According to CDC.gov BMI Calculator you are obese.";
                customer1.statusTitle = "Obese";
            }
            #endregion

            //xSubmitPopup.IsOpen = true;



            using (FileStream fileStream = new FileStream("yourBMI1.xml", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fileStream))
            using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.Indentation = 4;
                XmlSerializer ser = new XmlSerializer(typeof(Customer));
                ser.Serialize(xmlWriter, customer1);
                // ... Write elements
            }

        }
        private void xSubmitButtonOK_Click(object sender, RoutedEventArgs e)
        {
            xSubmitPopup.IsOpen = false;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void OnLoadStats()
        {
            Customer customer1 = new Customer();
            XmlSerializer des = new XmlSerializer(typeof(Customer));

            

            countDownDaXmls = numOfXmlsInUse;
            

            using (XmlReader reader = XmlReader.Create("yourBMI1.xml"))
            {
                customer1 = (Customer)des.Deserialize(reader);

                xLastNameBox.Text = customer1.lastName;
                xFirstNameBox.Text = customer1.firstName;
                xPhoneBox.Text = customer1.phoneNumber;
            }

            

            DataSet xmlData = new DataSet();
            xmlData.ReadXml("yourBMI1.xml");
            xDataGrid.DataSource = xmlData.Tables[0];
            
            

            whichTableBox = whichTableBox + 1;
            countDownDaXmls--;
            
            

            

        }
    }
}
