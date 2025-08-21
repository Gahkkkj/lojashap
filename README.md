# LojaShap - Loja Virtual em ASP.NET

Projeto de uma loja virtual desenvolvido com C# e o framework ASP.NET Core MVC. A aplicação simula um e-commerce de roupas e permite o gerenciamento completo de produtos, incluindo criação, edição, visualização e exclusão.

> 🚧 **Status:** Projeto concluído (desenvolvido para o curso Técnico em Programação para Web). 🚧

### 📸 Demonstração Visual

* **Link do canvas para demonstração da apresentação**
https://www.canva.com/design/DAGwq8Qigoc/TUWpJU2HXWasyxNeQR8QrQ/view?utm_content=DAGwq8Qigoc&utm_campaign=designshare&utm_medium=link2&utm_source=uniquelinks&utlId=he696c7a8d7
---

## ✨ Funcionalidades

* **Listagem de Produtos:** Visualização de todos os produtos disponíveis na loja em um layout de catálogo.
* **Página de Detalhes:** Cada produto possui uma página individual com mais informações.
* **Gerenciamento de Produtos (CRUD):**
    * ✔️ **Criar:** Adicionar novos produtos ao catálogo através de um formulário.
    * ✔️ **Editar:** Modificar informações de produtos já existentes.
    * ✔️ **Excluir:** Remover produtos do banco de dados.

---

## 🛠️ Tecnologias Utilizadas

O projeto foi construído utilizando as seguintes tecnologias:

* **Back-end:**
    * [C#](https://learn.microsoft.com/pt-br/dotnet/csharp/)
    * [ASP.NET Core MVC](https://learn.microsoft.com/pt-br/aspnet/core/mvc/overview?view=aspnetcore-6.0)
    * [Entity Framework Core](https://learn.microsoft.com/pt-br/ef/core/) (para a comunicação com o banco de dados)

* **Front-end:**
    * HTML5
    * CSS3
    * JavaScript
    * [Bootstrap](https://getbootstrap.com/) (para estilização e responsividade)

* **Banco de Dados:**
    * [SQL Server](https://www.microsoft.com/pt-br/sql-server/) (utilizado com o Entity Framework)

* **Ambiente de Desenvolvimento:**
    * Visual Studio 2022

---

## 🚀 Como Executar o Projeto

Siga os passos abaixo para rodar a aplicação localmente.

### Pré-requisitos

Antes de começar, você vai precisar ter instalado em sua máquina:
* [.NET 6.0 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/6.0) (ou superior)
* [Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/vs/) com a carga de trabalho "Desenvolvimento ASP.NET e para a Web"
* [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)

### Rodando o projeto

```bash
# 1. Clone este repositório
$ git clone [https://github.com/Gahkkkj/lojashap.git](https://github.com/Gahkkkj/lojashap.git)

# 2. Acesse a pasta do projeto
$ cd lojashap
```

3.  **Abra a solução** (`lojashap.sln`) no Visual Studio.

4.  **Configure o Banco de Dados:**
    * Abra o arquivo `appsettings.json`.
    * Localize a `ConnectionStrings` e altere o valor de `"DefaultConnection"` para apontar para a sua instância local do SQL Server. Geralmente, algo como:
        ```json
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LojaShapDB;Trusted_Connection=True;MultipleActiveResultSets=true"
        ```

5.  **Aplique as Migrations do Entity Framework:**
    * Abra o "Console do Gerenciador de Pacotes" no Visual Studio (`Exibir > Outras Janelas > Console do Gerenciador de Pacotes`).
    * Execute o comando abaixo para criar o banco de dados e as tabelas necessárias:
        ```powershell
        Update-Database
        ```

6.  **Execute a Aplicação:**
    * Pressione `F5` ou clique no botão "IIS Express" (com o ícone de play verde) no Visual Studio para iniciar o projeto.

---

## 👨‍💻 Autor

| [<img src="https://avatars.githubusercontent.com/u/104975550?v=4" width=115><br><sub>Gabriel da Silva Pereira</sub>](https://github.com/Gahkkkj) |
| :---: |

---

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE.md) para mais detalhes.
