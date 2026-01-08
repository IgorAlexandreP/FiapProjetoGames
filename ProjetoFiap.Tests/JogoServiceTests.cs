using Moq;
using ProjetoFiap.Application.Services;
using ProjetoFiap.Domain.Entities;
using ProjetoFiap.Domain.Interfaces;
using System;
using System.Collections.Generic;
using Xunit;

namespace ProjetoFiap.Tests
{
    public class JogoServiceTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly JogoService _jogoService;

        public JogoServiceTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _jogoService = new JogoService(_jogoRepositoryMock.Object);
        }

        [Fact]
        public void AdicionarJogo_ShouldCallRepository_WhenCalled()
        {
            // Arrange
            var jogo = new Jogo { Id = Guid.NewGuid(), Nome = "Game 1", Preco = 50 };

            // Act
            _jogoService.AdicionarJogo(jogo);

            // Assert
            _jogoRepositoryMock.Verify(r => r.Adicionar(jogo), Times.Once);
        }

        [Fact]
        public void ListarTodos_ShouldReturnGames_WhenCalled()
        {
            // Arrange
            var games = new List<Jogo>
            {
                new Jogo { Id = Guid.NewGuid(), Nome = "Game 1" },
                new Jogo { Id = Guid.NewGuid(), Nome = "Game 2" }
            };
            _jogoRepositoryMock.Setup(r => r.ObterTodos()).Returns(games);

            // Act
            var result = _jogoService.ListarTodos();

            // Assert
            Assert.Equal(games, result);
        }

        [Fact]
        public void ObterPorId_ShouldReturnGame_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var game = new Jogo { Id = id, Nome = "Game 1" };
            _jogoRepositoryMock.Setup(r => r.ObterPorId(id)).Returns(game);

            // Act
            var result = _jogoService.ObterPorId(id);

            // Assert
            Assert.Equal(game, result);
        }
    }
}
