using Moq;
using ProjetoFiap.Application.Services;
using ProjetoFiap.Domain.Entities;
using ProjetoFiap.Domain.Interfaces;
using System;
using Xunit;

namespace ProjetoFiap.Tests
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _usuarioService = new UsuarioService(_usuarioRepositoryMock.Object);
        }

        [Fact]
        public void RegistrarUsuario_ShouldRegister_WhenDataIsValid()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Test User",
                Email = "test@example.com",
                Senha = "Password123!"
            };

            // Act
            _usuarioService.RegistrarUsuario(usuario);

            // Assert
            _usuarioRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Once);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("test@")]
        [InlineData("@example.com")]
        public void RegistrarUsuario_ShouldThrow_WhenEmailIsInvalid(string invalidEmail)
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Test User",
                Email = invalidEmail,
                Senha = "Password123!"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _usuarioService.RegistrarUsuario(usuario));
            Assert.Equal("E-mail inválido.", exception.Message);
            _usuarioRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
        }

        [Theory]
        [InlineData("weak")]
        [InlineData("onlyletters")]
        [InlineData("12345678")]
        [InlineData("NoDigit!")]
        [InlineData("NoSpecial1")]
        public void RegistrarUsuario_ShouldThrow_WhenPasswordIsWeak(string weakPassword)
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Test User",
                Email = "test@example.com",
                Senha = weakPassword
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _usuarioService.RegistrarUsuario(usuario));
            Assert.Contains("Senha fraca", exception.Message);
            _usuarioRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
        }
    }
}
