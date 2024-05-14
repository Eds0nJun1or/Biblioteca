# Sistema de Gerenciamento de Biblioteca
## Descrição Geral

Este projeto consiste no desenvolvimento de um microserviço em C# para gerenciar uma biblioteca. O sistema oferece funcionalidades abrangentes para gerenciar livros, usuários, empréstimos, devoluções, multas e penalidades, atendendo às necessidades de bibliotecas de diferentes portes.

## Funcionalidades

O Sistema de Gerenciamento de Biblioteca oferece um conjunto abrangente de funcionalidades, incluindo:

## Gerenciamento de Livros:

* CRUD completo: Criação, leitura, atualização e exclusão de livros.
* Pesquisa por título, autor e categoria.
* Registro detalhado de informações de cada livro.
  
## Gerenciamento de Usuários:

* CRUD completo: Criação, leitura, atualização e exclusão de usuários.
* Cadastro de informações como nome, email e dados de contato.
* Histórico de empréstimos e devoluções para cada usuário.
  
## Gerenciamento de Empréstimos:

* Empréstimo de livros com restrição de até 3 livros por usuário.
* Registro da data de empréstimo e data prevista para devolução.
* Notificações por email para lembretes de devolução.
  
## Gerenciamento de Devoluções:

* Recebimento de livros emprestados.
* Cálculo de multas por atraso, se aplicável.
* Atualização do histórico de empréstimos e devoluções.
  
## Registro de Atividades:

* Registro detalhado de todas as atividades de empréstimo e devolução.
* Auditoria completa das operações realizadas no sistema.
  
## Funcionalidades Adicionais:

* API RESTful: Exposição de uma API RESTful para integração com outros sistemas e aplicativos.
* Segurança: Implementação de autenticação JWT para garantir o acesso seguro à API.
* Banco de Dados: Utilização do Entity Framework Core para integração com um banco de dados SQL.
* Validação de Dados: Validação rigorosa dos dados de entrada para garantir precisão e integridade.
* Testes Unitários: Desenvolvimento de testes unitários para assegurar a confiabilidade do código.
  
## Desafios Adicionais:

* Documentação da API: Documentação completa da API utilizando o Swagger para facilitar seu uso por outros desenvolvedores.
* Relatórios Avançados: Geração de relatórios sobre livros mais populares, usuários mais ativos, histórico de atrasos e outros indicadores relevantes.
* Funcionalidade de Multa Progressiva por Atraso: Multas crescentes por dia de atraso na devolução, com limite máximo de acordo com o valor do livro.
* Funcionalidade de Penalidades para Atrasos Recorrentes: Restrições para usuários com histórico de atrasos, como limitar o empréstimo a um livro por vez.
  
## Orientações para Implementação

A implementação do Sistema de Gerenciamento de Biblioteca deve seguir as seguintes etapas:

1. ## Estruturação do Projeto e Configuração do Ambiente de Desenvolvimento:
* Definir a estrutura do projeto e as ferramentas de desenvolvimento a serem utilizadas.
* Configurar o ambiente de desenvolvimento, incluindo banco de dados, IDE e ferramentas de versionamento de código.
  
2. ## Desenvolvimento das Funcionalidades Básicas:
* Implementar as funcionalidades CRUD para livros e usuários.
* Desenvolver as funcionalidades de empréstimo e devolução de livros.
* Integrar o sistema com um banco de dados SQL utilizando o Entity Framework Core.
  
3. ## Implementação das Funcionalidades Adicionais:
* Implementar a API RESTful e garantir a segurança com autenticação JWT.
* Aplicar validação de dados em todas as entradas do sistema.
* Desenvolver testes unitários para verificar a confiabilidade do código.
  
4. ## Desenvolvimento dos Desafios Adicionais:
* Documentar a API utilizando o Swagger para facilitar seu uso por outros desenvolvedores.
* Criar funcionalidades para geração de relatórios avançados.
* Implementar multas progressivas por atraso na devolução.
* Aplicar penalidades para usuários com histórico de atrasos recorrentes.

## Observações

* Este projeto oferece uma base sólida para o desenvolvimento de um Sistema de Gerenciamento de Biblioteca completo e funcional.
* As funcionalidades e desafios adicionais podem ser adaptados às necessidades específicas da biblioteca.
* A documentação detalhada e testes unitários garantem a qualidade e confiabilidade do sistema.

## Tecnologias Utilizadas

* C#
* Swagger
* API RESTful
* .NET Framework
* Autenticação JWT
* Banco de dados SQL
* Entity Framework Core
