using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CompuFlowGateway.TestApplication
{
    public partial class FormKeyValue : Form
    {
        public FormKeyValue()
        {
            InitializeComponent();
        }

        private string[] m_keys;
        public string[] Keys
        {
            get
            {
                return m_keys;
            }
        }

        private string[] m_values;
        public string[] Values
        {
            get
            {
                return m_values;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            m_keys = null;
            m_values = null;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SetKeysValues();
            this.Close();
        }

        private void SetKeysValues()
        {
            string[] sets = this.textBoxKeysValues.Text.Split(new char[] { '\n' });
            List<string> keys = new List<string>();
            List<string> values = new List<string>();

            foreach ( string set in sets ) {
                string[] pairs = set.Split(new char[] { '=' });
                if (pairs.Length != 2)
                {
                    continue;
                }
                keys.Add(pairs[0].Trim());
                values.Add(pairs[1].Trim());
            }
            m_keys = keys.ToArray<string>();
            m_values = values.ToArray<string>();
        }
    }
}
