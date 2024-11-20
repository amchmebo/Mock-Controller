using Microsoft.VisualStudio.TestTools.UnitTesting;
using mock.depart.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using mock.depart.Services;
using Microsoft.AspNetCore.Mvc;
using mock.depart.Models;

namespace mock.depart.Controllers.Tests
{
    [TestClass()]
    public class CatsControllerTests
    {
        [TestMethod()]
        public void Delete_CatNotFound()
        {
            //on MOCK le service
            Mock<CatsService> catServiceMock = new Mock<CatsService>();
            //on MOCK le contrôleur
            //option CallBase == mocker seulement une partie de l'objet (c.-à-d. le contrôleur)
            Mock<CatsController> catsController = new Mock<CatsController>(catServiceMock.Object) { CallBase = true };

            //fonction qui retourne null
            catServiceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(value: null);
            catsController.Setup(c => c.UserId).Returns("1"); //mauvais owner

            //on tente de supprimer le premier chat... qui n'existe pas
            var actionResult = catsController.Object.DeleteCat(0);

            var result = actionResult.Result as NotFoundResult;

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Delete_WrongUser()
        {
            Mock<CatsService> catServiceMock = new Mock<CatsService>();
            Mock<CatsController> catsController = new Mock<CatsController>(catServiceMock.Object) { CallBase = true };

            //créer un CatOwner
            CatOwner catOwner = new CatOwner()
            {
                Id = "11111"
            };
            //créer un chat
            Cat chat = new Cat()
            {
                Id = 1,
                Name = "Billie",
                CatOwner = catOwner,
                CuteLevel = Cuteness.Amazing
            };

            //on retourne le chat
            catServiceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(chat);
            //on retourne le mauvais owner
            catsController.Setup(c => c.UserId).Returns("1");

            var actionResult = catsController.Object.DeleteCat(0);

            var result = actionResult.Result as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Cat is not yours", result.Value);
        }

        [TestMethod()]
        public void Delete_CatTooCute()
        {
            Mock<CatsService> catServiceMock = new Mock<CatsService>();
            Mock<CatsController> catsController = new Mock<CatsController>(catServiceMock.Object) { CallBase = true };

            //créer un CatOwner
            CatOwner catOwner = new CatOwner()
            {
                Id = "11111"
            };
            //créer un chat
            Cat chat = new Cat()
            {
                Id = 1,
                Name = "Billie",
                CatOwner = catOwner,
                CuteLevel = Cuteness.Amazing
            };

            //on retourne le chat
            catServiceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(chat);
            //on retourne le bon owner
            catsController.Setup(c => c.UserId).Returns("11111");

            //on tente de supprimer le chat
            var actionResult = catsController.Object.DeleteCat(0);
            var result = actionResult.Result as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Cat is too cute", result.Value);

        }

        [TestMethod()]
        public void Delete_Success()
        {
            Mock<CatsService> catServiceMock = new Mock<CatsService>();
            Mock<CatsController> catsController = new Mock<CatsController>(catServiceMock.Object) { CallBase = true };

            //créer un CatOwner
            CatOwner catOwner = new CatOwner()
            {
                Id = "11111"
            };
            //créer un chat
            Cat chat = new Cat()
            {
                Id = 1,
                Name = "Billie",
                CatOwner = catOwner,
                CuteLevel = Cuteness.BarelyOk
            };

            //on retourne le chat
            catServiceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(chat);
            //on delete le chat
            catServiceMock.Setup(s => s.Delete(It.IsAny<int>())).Returns(chat);
            //on retourne le bon owner
            catsController.Setup(c => c.UserId).Returns("11111");

            var actionResult = catsController.Object.DeleteCat(0);

            var result = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(result);

            Cat? catResult = (Cat?)result!.Value;
            Assert.AreEqual(chat.Id, catResult!.Id);
        }
    }
}