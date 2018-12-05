﻿using System;
using System.Net;
using System.Threading.Tasks;
using ManagerLayer.Logic.PasswordChecking.PasswordValidations;
using DataAccessLayer;
using DataAccessLayer.PasswordChecking.HashFunctions;
using ServiceLayer.HttpClients;
using Xunit;
using ManagerLayer.Logic;
using ManagerLayer.BusinessRules;
using System.Collections.Generic;
using System.Diagnostics;

namespace ManagerLayerTests.Tests
{
    public class PasswordCheckerTests
    {
        // Arrange
        static SHA1HashFunction sha = new SHA1HashFunction();
        static string url = "https://api.pwnedpasswords.com/range/";
        static PwnedPasswordsValidation pv = new PwnedPasswordsValidation(sha, url);
        private IHttpClient HttpClientMethods = new HttpClientString(); // Http Client

        [Fact]
        public void PwnedPassword_FindHash_FoundShouldReturnCount()
        {
            // Arrange
            string hashValue = "1E4C9B93F3F0682250B6CF8331B7EE68FD8"; // password: "password"
            string response = "1D72CD07550416C216D8AD296BF5C0AE8E0:10 \n" +
                "1E2AAA439972480CEC7F16C795BBB429372:1 \n" +
                "1E3687A61BFCE35F69B7408158101C8E414:1 \n" +
                "1E4C9B93F3F0682250B6CF8331B7EE68FD8:3533661 \n";
            int expected = 3533661;
            int actual;

            //Act
            actual = pv.FindHash(hashValue, response);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PwnedPassword_FindHash_NotFoundShouldReturnZero()
        {
            // Arrange
            string hashValue = "753D006EBCE8F59C93364725A9D5C4EC6BC"; // password = "fw836g1"
            string response = "1D72CD07550416C216D8AD296BF5C0AE8E0:10 \n" +
                "1E2AAA439972480CEC7F16C795BBB429372:1 \n" +
                "1E3687A61BFCE35F69B7408158101C8E414:1 \n" +
                "1E4C9B93F3F0682250B6CF8331B7EE68FD8:3533661 \n";
            int expected = 0;
            int actual;

            //Act
            actual = pv.FindHash(hashValue, response);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PwnedPasswordValidation_FindHash_InvalidHashValueShouldThrowException()
        {
            // Arrange
            string response = "1D72CD07550416C216D8AD296BF5C0AE8E0: 10 \n" +
                "1E2AAA439972480CEC7F16C795BBB429372:1 \n" +
                "1E3687A61BFCE35F69B7408158101C8E414:1 \n" +
                "1E4C9B93F3F0682250B6CF8331B7EE68FD8:3533661 \n";
            Boolean expected = true;
            Boolean actual;

            //Act
            try
            {
                int test = pv.FindHash(null, response);
                actual = false;
            }
            catch (ArgumentNullException)
            {
                actual = true;
            }

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PwnedPasswordValidation_FindHash_InvalidResponseShouldThrowExecption()
        {
            // Arrange
            string hashValue = "1E4C9B93F3F0682250B6CF8331B7EE68FD8"; // password = "password"
            bool expected = true;
            bool actual;

            //Act
            try
            {
                int test = pv.FindHash(hashValue, null);
                actual = false;
            }
            catch (NullReferenceException)
            {
                actual = true;
            }

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PwnedPasswordValidation_FindHash_AllNullValuesShouldThrowException()
        {
            // Arrange
            bool expected = true;
            bool actual;

            //Act
            try
            {
                int test = pv.FindHash(null, null);
                actual = false;
            }
            catch (NullReferenceException)
            {
                actual = true;
            }

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HttpClientString_RequestData_ShouldPass()
        {
            // Arrange
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool expected = true;
            bool actual;

            //Act
            try
            {
                Uri uri = new Uri("https://api.pwnedpasswords.com/range/5BAA6");
                Task<string> response = HttpClientMethods.RequestData(uri);
                actual = true;
            }
            catch (WebException)
            {
                actual = false;
            }

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HttpClientString_RequestData_InvalidUrlShouldFail()
        {
            // Arrange
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool expected = true;
            bool actual;

            //Act
            try
            {
                Uri uri = new Uri("should throw exception");
                Task<string> response = HttpClientMethods.RequestData(uri);
            }
            catch (UriFormatException)
            {
                actual = true;
                //Assert
                Assert.Equal(expected, actual);
            }
        }

        //Data for PasswordCheckingBR_CheckPasswordCount_ShouldReturnValidStatus
        public static IEnumerable<object[]> GetPasswordStatusesData()
        {
            PasswordStatus e0 = new PasswordStatus(0); 
            PasswordStatus a0 = PasswordCheckingBR.CheckPasswordCount(0);
            yield return new object[] { e0, a0 };

            PasswordStatus e1 = new PasswordStatus(1);
            PasswordStatus a1 = PasswordCheckingBR.CheckPasswordCount(1);
            yield return new object[] { e1, a1 };

            PasswordStatus e2 = new PasswordStatus(2);
            PasswordStatus a2 = PasswordCheckingBR.CheckPasswordCount(2);
            yield return new object[] { e2, a2 };

            PasswordStatus e3 = new PasswordStatus(2);
            PasswordStatus a3 = PasswordCheckingBR.CheckPasswordCount(100);
            yield return new object[] { e3, a3 };

            PasswordStatus e4 = new PasswordStatus(2);
            PasswordStatus a4 = PasswordCheckingBR.CheckPasswordCount(-100);
            yield return new object[] { e4, a4 };
        }

        [Theory]
        //Arrange 
        [MemberData(nameof(GetPasswordStatusesData))]
        public void PasswordCheckingBR_CheckPasswordCount_ShouldReturnValidStatus(PasswordStatus e, PasswordStatus a)
        {
            // Act 
            int expected = e.status;
            int actual = a.status;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PwnedPasswordsValidation_Validate_ShouldBeFasterThanOneSecond()
        {
            // Arrange
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Act
            pv.Validate("password");
            stopwatch.Stop();

            // Assert
            Assert.True(stopwatch.ElapsedMilliseconds < 1000);
        }
    }
}