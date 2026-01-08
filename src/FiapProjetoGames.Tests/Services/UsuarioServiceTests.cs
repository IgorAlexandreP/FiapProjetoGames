using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FiapProjetoGames.Domain.Entities;
using FiapProjetoGames.Domain.Repositories;
using FiapProjetoGames.Application.Services;
using FiapProjetoGames.Application.DTOs;

namespace FiapProjetoGames.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _usuarioService = new UsuarioService(_mockUsuarioRepository.Object, "sua_chave_secreta_jwt");
        }

        [Fact]
        public async Task CadastrarAsync_ComDadosValidos_DeveRetornarUsuarioDto()
        {
            // Arrange
            var cadastroDto = new CadastroUsuarioDto
            {
                Nome = "Teste",
                Email = "teste@teste.com",
                Senha = "senha123",
                IsAdmin = false
            };

            _mockUsuarioRepository.Setup(repo => repo.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario)null);

            _mockUsuarioRepository.Setup(repo => repo.CriarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => u);

            // Act
            var resultado = await _usuarioService.CadastrarAsync(cadastroDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(cadastroDto.Nome, resultado.Nome);
            Assert.Equal(cadastroDto.Email, resultado.Email);
            Assert.Equal(cadastroDto.IsAdmin, resultado.IsAdmin);
            Assert.NotNull(resultado.Token);
        }

        [Fact]
        public async Task CadastrarAsync_ComEmailExistente_DeveLancarException()
        {
            // Arrange
            var cadastroDto = new CadastroUsuarioDto
            {
                Nome = "Teste",
                Email = "teste@teste.com",
                Senha = "senha123",
                IsAdmin = false
            };

            var usuarioExistente = new Usuario("Teste", "teste@teste.com", "hash", false);

            _mockUsuarioRepository.Setup(repo => repo.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(usuarioExistente);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _usuarioService.CadastrarAsync(cadastroDto)
            );
        }
    }
} 