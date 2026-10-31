using System;
using System.Collections.Generic;
using System.Linq;
using InterviewTestPagination.Models;
using InterviewTestPagination.Models.Todo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewTestPagination.Tests.Models
{
    [TestClass]
    public class TodoServiceTest
    {
        // Confere os valores da primeira página e a ordenação inicial por data decrescente.
        [TestMethod]
        public void List_PrimeiraPagina_DevolveVinteItens()
        {
            var result = new TodoService().List(1, 20, "createdDate", "desc");
            Assert.AreEqual(20, result.Items.Count);
            Assert.AreEqual(55, result.TotalItems);
            Assert.AreEqual(3, result.TotalPages);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(55L, result.Items[0].Id);
            Assert.AreEqual(36L, result.Items[19].Id);
        }

        // A segunda página precisa começar depois dos primeiros vinte registros.
        [TestMethod]
        public void List_SegundaPagina_DevolveOsProximosVinte()
        {
            var result = new TodoService().List(2, 20, "createdDate", "desc");
            Assert.AreEqual(20, result.Items.Count);
            Assert.AreEqual(35L, result.Items[0].Id);
            Assert.AreEqual(16L, result.Items[19].Id);
            Assert.AreEqual(2, result.Page);
        }

        // Uma página incompleta não pode perder os registros restantes.
        [TestMethod]
        public void List_UltimaPagina_DevolveQuinzeItens()
        {
            var result = new TodoService().List(3, 20, "createdDate", "desc");
            Assert.AreEqual(15, result.Items.Count);
            Assert.AreEqual(15L, result.Items[0].Id);
            Assert.AreEqual(1L, result.Items[14].Id);
        }

        // O número de páginas deve ser arredondado para cima.
        [TestMethod]
        public void List_DezPorPagina_CalculaSeisPaginas()
        {
            var result = new TodoService().List(6, 10, "id", "asc");
            Assert.AreEqual(6, result.TotalPages);
            Assert.AreEqual(5, result.Items.Count);
            Assert.AreEqual(51L, result.Items[0].Id);
        }

        [TestMethod]
        public void List_TrintaPorPagina_CalculaDuasPaginas()
        {
            var result = new TodoService().List(2, 30, "id", "asc");
            Assert.AreEqual(2, result.TotalPages);
            Assert.AreEqual(25, result.Items.Count);
            Assert.AreEqual(31L, result.Items[0].Id);
        }

        // Zero é o contrato escolhido para a opção 'all'.
        [TestMethod]
        public void List_Todos_DevolveOsCinquentaECincoRegistros()
        {
            var result = new TodoService().List(7, 0, "id", "desc");
            Assert.AreEqual(55, result.Items.Count);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(1, result.TotalPages);
            Assert.AreEqual(0, result.PageSize);
            Assert.AreEqual(55L, result.Items[0].Id);
            Assert.AreEqual(1L, result.Items[54].Id);
        }

        // Uma página muito alta é ajustada para a última, sem estourar o cálculo de Skip.
        [TestMethod]
        public void List_PaginaAcimaDoTotal_DevolveUltimaPagina()
        {
            var result = new TodoService().List(int.MaxValue, 20, "id", "asc");
            Assert.AreEqual(3, result.Page);
            Assert.AreEqual(15, result.Items.Count);
            Assert.AreEqual(41L, result.Items[0].Id);
        }

        // O mesmo contrato continua válido quando o repositório não tem registros.
        [TestMethod]
        public void List_SemRegistros_DevolvePaginaVazia()
        {
            var service = CreateService(new List<Todo>());
            var result = service.List(1, 20, "id", "asc");
            Assert.AreEqual(0, result.TotalItems);
            Assert.AreEqual(0, result.Items.Count);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(1, result.TotalPages);
        }

        [TestMethod]
        public void List_TodosSemRegistros_DevolvePaginaVazia()
        {
            var result = CreateService(new List<Todo>()).List(1, 0, "id", "asc");
            Assert.AreEqual(0, result.Items.Count);
            Assert.AreEqual(1, result.TotalPages);
        }

        // A ordenação deve ocorrer antes de selecionar a página.
        [TestMethod]
        public void List_IdCrescente_SegundaPaginaComecaNoVinteEUm()
        {
            var result = new TodoService().List(2, 20, "id", "asc");
            Assert.AreEqual(21L, result.Items[0].Id);
            Assert.AreEqual(40L, result.Items[19].Id);
        }

        // Dados fora de ordem permitem testar Task sem depender da ordem do repositório.
        [TestMethod]
        public void List_TaskCrescente_OrdenaPeloTexto()
        {
            var service = CreateService(CreateMixedTodos());
            var result = service.List(1, 10, "task", "asc");
            CollectionAssert.AreEqual(new long[] { 2, 1, 3 }, GetIds(result.Items));
        }

        [TestMethod]
        public void List_TaskDecrescente_OrdenaPeloTexto()
        {
            var service = CreateService(CreateMixedTodos());
            var result = service.List(1, 10, "task", "desc");
            CollectionAssert.AreEqual(new long[] { 3, 1, 2 }, GetIds(result.Items));
        }

        // As datas não seguem os Ids neste cenário, para testar a propriedade correta.
        [TestMethod]
        public void List_DataCrescente_OrdenaPelaData()
        {
            var service = CreateService(CreateMixedTodos());
            var result = service.List(1, 10, "createdDate", "asc");
            CollectionAssert.AreEqual(new long[] { 3, 1, 2 }, GetIds(result.Items));
        }

        [TestMethod]
        public void List_DataDecrescente_OrdenaPelaData()
        {
            var service = CreateService(CreateMixedTodos());
            var result = service.List(1, 10, "createdDate", "desc");
            CollectionAssert.AreEqual(new long[] { 2, 1, 3 }, GetIds(result.Items));
        }

        // O Id resolve empates para manter a ordem estável entre páginas.
        [TestMethod]
        public void List_TextosIguais_DesempataPeloId()
        {
            var todos = new List<Todo>
            {
                new Todo(3, "Same", new DateTime(2020, 1, 1)),
                new Todo(1, "same", new DateTime(2020, 1, 1)),
                new Todo(2, "SAME", new DateTime(2020, 1, 1))
            };
            var service = CreateService(todos);
            var result = service.List(1, 10, "task", "asc");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, GetIds(result.Items));
        }

        [TestMethod]
        public void List_DatasIguais_DesempataPeloId()
        {
            var todos = new List<Todo>
            {
                new Todo(2, "B", new DateTime(2020, 1, 1)),
                new Todo(1, "A", new DateTime(2020, 1, 1))
            };
            var result = CreateService(todos).List(1, 10, "createdDate", "desc");
            CollectionAssert.AreEqual(new long[] { 1, 2 }, GetIds(result.Items));
        }

        // As validações devem rejeitar opções que não fazem parte do contrato.
        [TestMethod]
        public void List_PaginaZero_GeraErroDeValidacao()
        {
            Assert.ThrowsException<ArgumentException>(() => new TodoService().List(0, 20, "id", "asc"));
        }

        [TestMethod]
        public void List_PaginaNegativa_GeraErroDeValidacao()
        {
            Assert.ThrowsException<ArgumentException>(() => new TodoService().List(-1, 20, "id", "asc"));
        }

        [TestMethod]
        public void List_TamanhoInvalido_GeraErroDeValidacao()
        {
            Assert.ThrowsException<ArgumentException>(() => new TodoService().List(1, 15, "id", "asc"));
            Assert.ThrowsException<ArgumentException>(() => new TodoService().List(1, -1, "id", "asc"));
        }

        [TestMethod]
        public void List_CampoInvalido_GeraErroDeValidacao()
        {
            Assert.ThrowsException<ArgumentException>(() => new TodoService().List(1, 20, "unknown", "asc"));
        }

        [TestMethod]
        public void List_DirecaoInvalida_GeraErroDeValidacao()
        {
            Assert.ThrowsException<ArgumentException>(() => new TodoService().List(1, 20, "id", "other"));
        }

        // Paginar não deve criar novos objetos no lugar dos registros originais.
        [TestMethod]
        public void List_MantemOsObjetosDoRepositorio()
        {
            var todos = CreateMixedTodos();
            var result = CreateService(todos).List(1, 10, "id", "asc");
            Assert.AreSame(todos[0], result.Items[0]);
        }

        // Navegar por todas as páginas não pode repetir ou perder tarefas.
        [TestMethod]
        public void List_TodasAsPaginas_CobremOsRegistrosSemRepeticao()
        {
            var service = new TodoService();
            var ids = new List<long>();
            for (var page = 1; page <= 3; page++)
                ids.AddRange(GetIds(service.List(page, 20, "id", "asc").Items));

            Assert.AreEqual(55, ids.Count);
            Assert.AreEqual(55, ids.Distinct().Count());
            Assert.AreEqual(1L, ids[0]);
            Assert.AreEqual(55L, ids[54]);
        }

        // Os auxiliares abaixo só preparam dados controlados para os testes.
        private static List<Todo> CreateMixedTodos()
        {
            return new List<Todo>
            {
                new Todo(1, "Bravo", new DateTime(2020, 1, 2)),
                new Todo(2, "alpha", new DateTime(2020, 1, 3)),
                new Todo(3, "Charlie", new DateTime(2020, 1, 1))
            };
        }

        private static long[] GetIds(List<Todo> todos)
        {
            return todos.Select(todo => todo.Id).ToArray();
        }

        private static TodoService CreateService(List<Todo> todos)
        {
            var service = new TodoService();
            service.Repository = new TestRepository(todos);
            return service;
        }

        private class TestRepository : IModelRepository<Todo>
        {
            private readonly List<Todo> _todos;

            public TestRepository(List<Todo> todos)
            {
                _todos = todos;
            }

            public IEnumerable<Todo> All()
            {
                return _todos;
            }
        }
    }
}
