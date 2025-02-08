using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace TesteCervantes
{
    public partial class Form1 : Form
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=Js2bk;Database=cadastro_app_db";
        public Form1()
        {
            InitializeComponent();
        }

        private void cadastrar_Click(object sender, EventArgs e)
        {
            //string text = textInput.Text;
            //int number = Convert.ToInt32(numberInput.Text);

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO cadastro (texto, numero) VALUES (@texto, @numero);";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@texto", textInput.Text);
                        command.Parameters.AddWithValue("@numero", Convert.ToInt32(numberInput.Text));
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cadastro realizado com sucesso!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private void LoadData()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT texto, numero FROM cadastro;";
                    using (var data = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        data.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void textInput_Validating(object sender, CancelEventArgs e)
        {
            //verifica se o campo do input não está vazio
            if (String.IsNullOrEmpty(textInput.Text)) 
            {
                setError(textInput, errorProvider1, "Este campo é obrigatório!", e);
            }
            else
            {
                clearError(textInput, errorProvider1);
            }
        }

        private void numberInput_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(numberInput.Text))
            {
                setError(numberInput, errorProvider2, "Este campo é obrigatório!", e);
                return;
            }

            
            if (!int.TryParse(numberInput.Text, out _))
            {
                setError(numberInput, errorProvider2, "Este campo deve somente conter números", e);
                return;
            }

            if (numberInput.TextLength < 1)
            {
                setError(numberInput, errorProvider2, "Deve haver no mínimo dois números", e);
                return;
            }

            clearError(numberInput, errorProvider2);
        }

        private void editar_Click(object sender, EventArgs e)
        {
            int numero; 

            //realiza validação dos campos;
            if(string.IsNullOrWhiteSpace(textInput.Text) || string.IsNullOrWhiteSpace(numberInput.Text))
            {
                MessageBox.Show("Todos os campos devem estar preenchidos!");
                return;
            }

            if(!int.TryParse(numberInput.Text, out numero) || numero <= 1 )
            {
                MessageBox.Show("O número deve conter no mínimo 2 números");
                return;
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string updateQuery = "UPDATE cadastro SET texto = @texto WHERE numero = @numero AND texto <> @texto;";

                    using (var updateCommand = new NpgsqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@texto", textInput.Text);
                        updateCommand.Parameters.AddWithValue("@numero", numero);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Dados atualizados com sucesso!");
                            LoadData();
                        } 
                        else
                        {
                            MessageBox.Show("Nenhum dado foi atualizado!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao atualizar: " + ex.Message);
                }
            }
        }


        //método que lança o erro na hora da validação e altera o estilo dos inputs
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textInput.Text = row.Cells["texto"].Value.ToString();
                numberInput.Text = row.Cells["numero"].Value.ToString();
            }
        }

        
    }
}
