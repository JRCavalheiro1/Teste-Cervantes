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
        //declara a string de conexão com o banco de dados
        private string connectionString = "Host=localhost;Username=user;Password=password;Database=seu_db";
        public Form1()
        {
            InitializeComponent();
        }

        private void register_Click(object sender, EventArgs e)
        {
            //string text = textInput.Text;
            int number = Convert.ToInt32(numberInput.Text);

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO cadastro (texto, numero) VALUES (@texto, @numero);";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@texto", textInput.Text);
                        command.Parameters.AddWithValue("@numero", number);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cadastro realizado com sucesso!");
                    registerLog("Insert", number);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private void edit_Click(object sender, EventArgs e)
        {
            int number;

            //realiza validação dos campos;
            if (string.IsNullOrWhiteSpace(textInput.Text) || string.IsNullOrWhiteSpace(numberInput.Text))
            {
                MessageBox.Show("Todos os campos devem estar preenchidos!");
                return;
            }

            if (!int.TryParse(numberInput.Text, out number) || number <= 1)
            {
                MessageBox.Show("O número deve conter no mínimo 2 números");
                return;
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    //verifica se o número digitado existe no banco de dados
                    string checkQuery = "SELECT COUNT(*) FROM cadastro WHERE numero = @numero;";

                    using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@numero", number);
                        int numberExists = Convert.ToInt32(checkCommand.ExecuteScalar());
                        if (numberExists == 0)
                        {
                            MessageBox.Show("Número não econtrado no registro");
                            return;
                        }
                    }

                    //verifica se o texto digitado não é o mesmo já registrado
                    string selectQuery = "SELECT texto FROM cadastro WHERE numero = @numero;";
                    using (var selectCommand = new NpgsqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@texto", textInput.Text);
                        var currentText = selectCommand.ExecuteScalar()?.ToString();

                        if (currentText == textInput.Text)
                        {
                            MessageBox.Show("Os dados já estão cadastrados com esse valor.");
                            return;
                        }
                    }

                    //altera o valor do texto no banco de dados 
                    string updateQuery = "UPDATE cadastro SET texto = @texto WHERE numero = @numero;";

                    using (var updateCommand = new NpgsqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@texto", textInput.Text);
                        updateCommand.Parameters.AddWithValue("@numero", number);
                        

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Dados atualizados com sucesso!");
                            registerLog("Uptade", number);
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

        private void delete_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecione um registro para deletar");
                return; 
            }

            int selectedNumber = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["numero"].Value);

            DialogResult confirmMessage = MessageBox.Show(
                "Você tem certeza que deseja excluir este registro?",
                "Cofirme",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if( confirmMessage == DialogResult.No )
            {
                return;
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM cadastro WHERE numero = @numero";

                    using (var deleteCommand = new NpgsqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@numero", selectedNumber);
                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if( rowsAffected > 0 )
                        {
                            registerLog("Delete", selectedNumber);
                            MessageBox.Show("Registro deletado com sucesso");
                            LoadData();
                        } 
                        else
                        {
                            MessageBox.Show("Erro ao deletar registro");
                        }
                    }

                } 
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao deletar: ", ex.Message);
                }

            }

        }

        //método que carrega os dados do banco de dados para exibir no grid view
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

       private void registerLog(string operationType, int number)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string registerQuery = "INSERT INTO log_operacoes (operacao, numero) VALUES (@operacao, @numero);";

                    using (var command = new NpgsqlCommand(registerQuery, connection))
                    {
                        command.Parameters.AddWithValue("@operacao", operationType);
                        command.Parameters.AddWithValue("@numero", number);
                        command.ExecuteNonQuery();
                    }
                } 
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao registrar log: ", ex.Message);
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
