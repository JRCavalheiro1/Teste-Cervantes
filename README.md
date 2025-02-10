# Teste-Cervantes

Aplicação desktop de cadastro utilizando Windows Forms (C#) com PostgreSQL

## Considerações

### Criação das tabelas
  1. Verifique se você possui o PostgreSQL instalado na sua maquina.
  2. Conecte o seu banco de dados pelo PgAdmin ou pelo terminal.
  3. Execute o arquivo `cadastroApp.sql` para a criação das tabelas.
```sh
psql -U seu_usuario -d seu_banco_de_dados -a -f cadastroApp.sql
```
### Execução da aplicação
  1. Abra o arquivo solução `TesteCervantes.sln` no Visual Studio.
  2. Configue a string de conexão `connectionString` na **linha 17** do código da aplicação.
```C#
  private string connectionString = "Host=localhost;Username=user;Password=password;Database=seu_db";
```
  3. Compile e execute a aplicação.
