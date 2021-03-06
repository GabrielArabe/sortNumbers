# Sort Numbers
**Intro:**
	Essa solução faz requisições em todas as páginas da API disposta por uma empresa (http://challenge.dienekes.com.br/api/numbers?page={pageVar}), a cada requisição a lista de doubles é inserida numa lista. Após receber todas as páginas, é utilizado um algorítmo (Quick Sort) para ordenar os valores da lista completa do menor para o maior. Por fim, essa lista ordenada é exposta numa API (https://sortnumbers.azurewebsites.net/api/sortednumbers).

**Como testar**
A aplicação está hosteada na Azure, tendo a documentação do **Swagger** para indicar a rota https://sortnumbers.azurewebsites.net/swagger do Get. A requisição retorna uma lista ordenada de 1 milhão de números, por isso é provável que o Swagger não consiga apresentar o valor, é mais indicado fazer essa requisição (https://sortnumbers.azurewebsites.net/api/sortednumbers) utilizando o Postman ou, se não for possível, o Browser.

**Detalhes Técnicos**

Por ser um processo demorado, devido as diversas consultas na API foram tomadas algumas medidas para melhorar a performance: 

* O processo de consulta a API foi feito com Multithreading com o número de núcleos do processador;
* Foi criado um cache em memória com 10 minutos de expiração para a API retornar o valor processado anteriormente;
* Caso a requisição da API do sortnumbers ainda esteja sendo processada, e seja feita outra requisição, é retornada a mensagem de que a requisição já está sendo processada, para tentar novamente em breve.

Foram criados testes unitários para verificar as respostas:
* Da API
* Da ordenação  

Criado sistema de logs, atualmente salvos em disco e no player Datadog.

**Sobre o Datadog**

**Obs:** Criei uma conta teste para ter uma visualização melhor dos logs.
Ao executar a aplicação, os logs serão gravados com os níveis: Info, Warn e Error.

**Acesso ao datadog:**
* link: https://app.datadoghq.com/account/login?redirect=f
* usuário: gabriel@arabesaraiva.com
* senha: GabrielArabe123
* link área dos logs: https://app.datadoghq.com/logs

**Melhorias e Próximos passos**
* Adicionar mais testes unitários;
* Adicionar mais tratamentos de exceções; 
* Melhoria dos logs para exibir de forma mais útil no Datadog;
* Criar documentação com Postman.
