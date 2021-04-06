using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PlexureAPITest
{
    [TestFixture]
    public class Test
    {
        Service service;

        [OneTimeSetUp]
        public void Setup()
        {
            service = new Service();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            if (service != null)
            {
                service.Dispose();
                service = null;
            }
        }

        [Test]
        public void TEST_001_Login_With_Valid_User()
        {
            var response = service.Login("Tester", "Plexure123");
            response.Expect(HttpStatusCode.OK);

            //Validate values of the json response body
            Assert.AreEqual(response.Entity.UserName, "Tester");
            Assert.AreEqual(response.Entity.UserId, 1);
            Assert.AreEqual(response.Entity.AccessToken, "37cb9e58-99db-423c-9da5-42d5627614c5");
          
        }

       [Test]
        public void TEST_003_Login_With_Invalid_Username()
        {
            var response = service.Login("Testar", "Plexure123");
            response.Expect(HttpStatusCode.Unauthorized);

            //Validate if json response body contains correct error message
            Assert.AreEqual(response.Error, "\"Error: Unauthorized\"");
        }

        [Test]
        public void TEST_004_Login_With_Incorrect_Password()
        {
            var response = service.Login("Tester", "Plexure132");
            response.Expect(HttpStatusCode.Unauthorized);

            //Validate if json response body contains correct error message
            Assert.AreEqual(response.Error, "\"Error: Unauthorized\"");
        }

        [Test]
        public void TEST_005_Login_With_Missing_Both_Username_Password()
        {
            var response = service.Login("", "");
            response.Expect(HttpStatusCode.BadRequest);

            //Validate if json response body contains correct error message
            Assert.AreEqual(response.Error, "\"Error: Username and password required.\"");

        }

        [Test]
        public void TEST_006_Login_With_Missing_Username()
        {
            var response = service.Login("", "Plexure123");
            response.Expect(HttpStatusCode.Unauthorized);

            //Validate if json response body contains correct error message
            Assert.AreEqual(response.Error, "\"Error: Unauthorized\"");

        }

        [Test]
        public void TEST_007_Login_With_Missing_Password()
        {
            var response = service.Login("Tester", "");
            response.Expect(HttpStatusCode.Unauthorized);

            //Validate if json response body contains correct error message
            Assert.AreEqual(response.Error, "\"Error: Unauthorized\"");

        }


        [Test]
        public void TEST_008_Get_Points_For_Logged_In_User()
        {
            var points = service.GetPoints();
            points.Expect(HttpStatusCode.Accepted);

            //Validate values of the json response body
            Assert.AreEqual(points.Entity.UserId, 1);
            Assert.IsNotNull(points.Entity.Value);

        }


        [Test]
        public void TEST_009_Purchase_Product()
        {
            //Get Points prior to purchasing
            var points1 = service.GetPoints();
            int points_beforepurchase = points1.Entity.Value;

            //Validate Response Body after purchasing
            int productId = 1;
            var response = service.Purchase(productId);
            response.Expect(HttpStatusCode.Accepted);
            Assert.AreEqual(response.Entity.Message, "Purchase completed.");
            Assert.AreEqual(response.Entity.Points, 100);

            //Get Points after purchasing and compare if it increases by 100 points
            var points2 = service.GetPoints();
            int points_afterpurchase = points2.Entity.Value;
            Assert.AreEqual((points_beforepurchase + 100), points_afterpurchase) ;

        }

        [Test]
        public void TEST_010_Purchase_NonExisting_Product()
        {
            int productId = 0;
            var response = service.Purchase(productId);
            response.Expect(HttpStatusCode.BadRequest);

            //Validate if json response body contains correct error message
            Assert.AreEqual(response.Error, "\"Error: Invalid product id\"");
            
        }


    }
}
