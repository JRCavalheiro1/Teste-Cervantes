using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesteCervantes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            String userName = nameInput.Text;
            int userPassword = Convert.ToInt32(passwordInput.Text);
        }

        private void nameInput_Validating(object sender, CancelEventArgs e)
        {
            if(String.IsNullOrEmpty(nameInput.Text))
            {
                setError(nameInput, errorProvider1, "Este campo é obrigatório!", e);
            }
            else
            {
                clearError(nameInput, errorProvider1);
            }
        }

        private void passwordInput_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(passwordInput.Text))
            {
                setError(passwordInput, errorProvider2, "Este campo é obrigatório!", e);
                return;
            }


            if (!int.TryParse(passwordInput.Text, out _))
            {
                setError(passwordInput, errorProvider2, "A senha deve somente conter números", e);
                return;
            }

            if (passwordInput.TextLength < 2)
            {
                setError(passwordInput, errorProvider2, "A senha deve ter pelo menos 2 caracteres!", e);
                return;
            }

            clearError(passwordInput, errorProvider2);
        }

        private void setError(System.Windows.Forms.TextBox input, ErrorProvider provider, String message, CancelEventArgs e)
        {
            provider.SetError(input, message);
            input.BackColor = Color.IndianRed;
            input.ForeColor = Color.White;
            e.Cancel = true;
        }

        private void clearError(System.Windows.Forms.TextBox input, ErrorProvider provider)
        {
            provider.SetError(input, "");
            input.BackColor = Color.White;
            input.ForeColor = Color.Black;
        }

    }
}
