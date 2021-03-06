using DataLayer;
using DomainLibrary.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class ImplementationTests
    {
        [TestMethod]
        public void TestMultipleAdditionsAndGetAllRunningSessions()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            int totalAdded = 0;

            //Act
            for (int i = 0; i < 1000; i++)
            {
                m.AddRunningTraining(DateTime.Now, 10000, new TimeSpan(1, 0, 0), null, TrainingType.Recuperation, null);
                totalAdded++;
            }
            var collection = m.GetAllRunningSessions();

            //Assert
            Assert.IsTrue(collection.Count == totalAdded);
            Assert.IsTrue(m.GetAllCyclingSessions().Count == 0,"There were Cycling Sessions added");
        }
        [TestMethod]
        public void TestMultipleAdditionsAndGetAllCyclingSessions()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            int totalAdded = 0;

            //Act
            for (int i = 0; i < 1000; i++)
            {
                m.AddCyclingTraining(DateTime.Now, 10, new TimeSpan(1, 0, 0), null,null, TrainingType.Recuperation, null,BikeType.CityBike);
                totalAdded++;
            }
            var collection = m.GetAllCyclingSessions();

            //Assert
            Assert.IsTrue(collection.Count == totalAdded);
            Assert.IsTrue(m.GetAllRunningSessions().Count == 0,"There were Running Sessions added");
        }
        [TestMethod]
        public void TestNullAcceptanceForCyclingSessionTable()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            m.AddCyclingTraining(DateTime.Now, null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.CityBike);

            //Act
            CyclingSession cyclingSession = m.GetPreviousCyclingSessions(1)[0];

            //Assert
            Assert.IsNull(cyclingSession.AverageSpeed);
            Assert.IsNull(cyclingSession.Distance);
            Assert.IsNull(cyclingSession.AverageWatt);
            Assert.IsNull(cyclingSession.Comments);
        }
        public void TestNullAcceptanceForRunningSessionTable()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            m.AddRunningTraining(DateTime.Now, 15000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, null);

            //Act
            RunningSession runningSession = m.GetPreviousRunningSessions(1)[0];

            //Assert
            Assert.IsNull(runningSession.Comments);
        }
        [TestMethod]
        public void TestAddCyclingSessionToDatabase()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            DateTime now = DateTime.Now;
            CyclingSession cS = new CyclingSession(now, 10, new TimeSpan(2, 0, 0),null, null, TrainingType.Endurance, "", BikeType.CityBike);
            //Act
            m.AddCyclingTraining(now, 10, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, "", BikeType.CityBike);
            CyclingSession retrieved = m.GetPreviousCyclingSessions(1)[0];

            //Assert
            Assert.IsTrue(m.GetAllCyclingSessions().Count == 1);
            Assert.IsTrue(m.GetAllRunningSessions().Count == 0, "Cycling Session got added as Running Session as well");
            Assert.AreEqual(retrieved.AverageSpeed, cS.AverageSpeed,"AverageSpeed did not match up");
            Assert.AreEqual(retrieved.AverageWatt, cS.AverageWatt, "AverageWatt did not match up");
            Assert.AreEqual(retrieved.BikeType, cS.BikeType, "BikeType did not match up");
            Assert.AreEqual(retrieved.Comments, cS.Comments, "Comments did not match up");
            Assert.AreEqual(retrieved.Distance, cS.Distance, "Distance did not match up");
            Assert.AreEqual(retrieved.Time, cS.Time, "Time did not match up");
            Assert.AreEqual(retrieved.TrainingType, cS.TrainingType, "TrainingType did not match up");
            Assert.AreEqual(retrieved.When, cS.When, "When did not match up");
        }
        [TestMethod]
        public void TestAddRunningSessionToDatabase()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));

            DateTime now = DateTime.Now;
            RunningSession cS = new RunningSession(now, 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, "");
            //Act
            m.AddRunningTraining(now, 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, "");
            RunningSession retrieved = m.GetPreviousRunningSessions(1)[0];

            //Assert
            Assert.IsTrue(m.GetAllRunningSessions().Count == 1);
            Assert.IsTrue(m.GetAllCyclingSessions().Count == 0, "Running Session got added as Cycling Session as well");
            Assert.AreEqual(retrieved.AverageSpeed, cS.AverageSpeed, "AverageSpeed did not match up");
            Assert.AreEqual(retrieved.Comments, cS.Comments, "Comments did not match up");
            Assert.AreEqual(retrieved.Distance, cS.Distance, "Distance did not match up");
            Assert.AreEqual(retrieved.Time, cS.Time, "Time did not match up");
            Assert.AreEqual(retrieved.TrainingType, cS.TrainingType, "TrainingType did not match up");
            Assert.AreEqual(retrieved.When, cS.When, "When did not match up");
        }
        [TestMethod]
        public void TestRemovingRunningSessionFromDatabase()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            DateTime now = DateTime.Now;
            RunningSession runSession = new RunningSession(now, 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, "");
            RunningSession toDeleteRunningSession = new RunningSession(new DateTime(2010, 5, 12), 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, null);
            m.AddRunningTraining(toDeleteRunningSession.When, toDeleteRunningSession.Distance, toDeleteRunningSession.Time, toDeleteRunningSession.AverageSpeed, toDeleteRunningSession.TrainingType, toDeleteRunningSession.Comments);
            m.AddRunningTraining(runSession.When,runSession.Distance,runSession.Time,runSession.AverageSpeed,runSession.TrainingType,runSession.Comments);
            TrainingManager m2 = new TrainingManager(new UnitOfWork(new TrainingContextTest(true)));

            //Act
            m2.RemoveTrainings(new List<int>(), new List<int> { 1 });
            var runningSessions = m.GetAllRunningSessions();
            RunningSession retrievedRun = runningSessions[0];

            //Assert
            Assert.IsTrue(runningSessions.Count == 1, "The Running Session was not deleted");
            Assert.AreEqual(retrievedRun.AverageSpeed, runSession.AverageSpeed, "AverageSpeed did not match up");
            Assert.AreEqual(retrievedRun.Comments, runSession.Comments, "Comments did not match up");
            Assert.AreEqual(retrievedRun.Distance, runSession.Distance, "Distance did not match up");
            Assert.AreEqual(retrievedRun.Time, runSession.Time, "Time did not match up");
            Assert.AreEqual(retrievedRun.TrainingType, runSession.TrainingType, "TrainingType did not match up");
            Assert.AreEqual(retrievedRun.When, runSession.When, "When did not match up");
        }
        [TestMethod]
        public void TestRemovingCyclingSessionFromDatabase()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            DateTime now = DateTime.Now;
            CyclingSession cS = new CyclingSession(now, null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.CityBike);
            CyclingSession ToDeleteSessions = new CyclingSession(new DateTime(2010, 5, 12), null, new TimeSpan(2, 0, 0), null,null, TrainingType.Endurance, null,BikeType.IndoorBike);
            m.AddCyclingTraining(ToDeleteSessions.When, ToDeleteSessions.Distance, ToDeleteSessions.Time, ToDeleteSessions.AverageSpeed, ToDeleteSessions.AverageWatt, ToDeleteSessions.TrainingType, ToDeleteSessions.Comments, ToDeleteSessions.BikeType);
            m.AddCyclingTraining(cS.When, cS.Distance, cS.Time, cS.AverageSpeed, cS.AverageWatt, cS.TrainingType, cS.Comments, cS.BikeType);

            //Act
            TrainingManager m2 = new TrainingManager(new UnitOfWork(new TrainingContextTest(true)));
            m2.RemoveTrainings(new List<int>() { 1 }, new List<int>());
            var cyclingSessions = m2.GetAllCyclingSessions();
            CyclingSession retrievedCycle = cyclingSessions[0];

            //Assert
            Assert.IsTrue(cyclingSessions.Count == 1, "The Running Session was not deleted");
            Assert.AreEqual(retrievedCycle.AverageSpeed, cS.AverageSpeed, "AverageSpeed did not match up");
            Assert.AreEqual(retrievedCycle.Comments, cS.Comments, "Comments did not match up");
            Assert.AreEqual(retrievedCycle.Distance, cS.Distance, "Distance did not match up");
            Assert.AreEqual(retrievedCycle.Time, cS.Time, "Time did not match up");
            Assert.AreEqual(retrievedCycle.TrainingType, cS.TrainingType, "TrainingType did not match up");
            Assert.AreEqual(retrievedCycle.When, cS.When, "When did not match up");
            Assert.AreEqual(retrievedCycle.BikeType, cS.BikeType, "BikeType did not match up");
        }
        [TestMethod]
        public void TestRemovingMixedSessionFromDatabase()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            DateTime now = DateTime.Now;
            CyclingSession cS = new CyclingSession(now, null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.CityBike);
            CyclingSession ToDeleteSessions = new CyclingSession(new DateTime(2010, 5, 12), null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.IndoorBike);
            RunningSession runSession = new RunningSession(now, 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, "");
            RunningSession toDeleteRunningSession = new RunningSession(new DateTime(2010, 5, 12), 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, null);
            m.AddCyclingTraining(ToDeleteSessions.When, ToDeleteSessions.Distance, ToDeleteSessions.Time, ToDeleteSessions.AverageSpeed, ToDeleteSessions.AverageWatt, ToDeleteSessions.TrainingType, ToDeleteSessions.Comments, ToDeleteSessions.BikeType);
            m.AddCyclingTraining(cS.When, cS.Distance, cS.Time, cS.AverageSpeed, cS.AverageWatt, cS.TrainingType, cS.Comments, cS.BikeType);
            m.AddRunningTraining(toDeleteRunningSession.When, toDeleteRunningSession.Distance, toDeleteRunningSession.Time, toDeleteRunningSession.AverageSpeed, toDeleteRunningSession.TrainingType, toDeleteRunningSession.Comments);
            m.AddRunningTraining(runSession.When, runSession.Distance, runSession.Time, runSession.AverageSpeed, runSession.TrainingType, runSession.Comments);
            TrainingManager m2 = new TrainingManager(new UnitOfWork(new TrainingContextTest(true)));

            //Act
            m2.RemoveTrainings(new List<int>() { 1 }, new List<int>() {1 });
            var cyclingSessions = m2.GetAllCyclingSessions();
            var runningSessions = m.GetAllRunningSessions();
            CyclingSession RetrievedCycle = cyclingSessions[0];
            RunningSession retrievedRun = runningSessions[0];

            //Assert
            Assert.IsTrue(cyclingSessions.Count == 1, "The Running Session was not deleted");
            Assert.IsTrue(runningSessions.Count == 1, "The Running Session was not deleted");

            Assert.AreEqual(RetrievedCycle.AverageSpeed, cS.AverageSpeed, "AverageSpeed did not match up");
            Assert.AreEqual(RetrievedCycle.Comments, cS.Comments, "Comments did not match up");
            Assert.AreEqual(RetrievedCycle.Distance, cS.Distance, "Distance did not match up");
            Assert.AreEqual(RetrievedCycle.Time, cS.Time, "Time did not match up");
            Assert.AreEqual(RetrievedCycle.TrainingType, cS.TrainingType, "TrainingType did not match up");
            Assert.AreEqual(RetrievedCycle.When, cS.When, "When did not match up");
            Assert.AreEqual(RetrievedCycle.BikeType, cS.BikeType, "BikeType did not match up");

            Assert.AreEqual(retrievedRun.AverageSpeed, runSession.AverageSpeed, "AverageSpeed did not match up");
            Assert.AreEqual(retrievedRun.Comments, runSession.Comments, "Comments did not match up");
            Assert.AreEqual(retrievedRun.Distance, runSession.Distance, "Distance did not match up");
            Assert.AreEqual(retrievedRun.Time, runSession.Time, "Time did not match up");
            Assert.AreEqual(retrievedRun.TrainingType, runSession.TrainingType, "TrainingType did not match up");
            Assert.AreEqual(retrievedRun.When, runSession.When, "When did not match up");
        }
        [TestMethod]
        public void TestFindLatestCyclingSessionEnoughInDatabase()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            DateTime first = new DateTime(2000, 5, 12);
            DateTime second = new DateTime(2008, 5, 12);
            DateTime third = DateTime.Now;

            m.AddCyclingTraining(second, null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.CityBike);
            m.AddCyclingTraining(first, null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.IndoorBike);
            m.AddCyclingTraining(third, null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.MountainBike);
            m.AddRunningTraining(DateTime.Now, 2000, new TimeSpan(1, 0, 0), null, TrainingType.Endurance, null);

            //Act
            var collection = m.GetPreviousCyclingSessions(2);

            //Assert
            Assert.IsTrue(collection.Count == 2, "Did not return the right amount amount");
            Assert.IsTrue(collection[0].When == first,"Did not get returned in the right order");
            Assert.IsTrue(collection[1].When == second, "Did not get returned in the right order");
        }

        [TestMethod]
        public void TestFindSingleLatestRunningSessionEnoughInDatabase()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            DateTime first = new DateTime(2000, 5, 12);
            DateTime second = new DateTime(2008, 5, 12);
            DateTime third = DateTime.Now;
            
            m.AddRunningTraining(second, 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, null);
            m.AddRunningTraining(first, 10000, new TimeSpan(2, 0, 0),null, TrainingType.Endurance, null);
            m.AddRunningTraining(third, 10000, new TimeSpan(2, 0, 0),null, TrainingType.Endurance, null);
            m.AddCyclingTraining(DateTime.Now, 20, new TimeSpan(1, 0, 0), null,null, TrainingType.Endurance, null,BikeType.MountainBike);

            //Act
            var collection = m.GetPreviousRunningSessions(2);

            //Assert
            Assert.IsTrue(collection.Count == 2, "Did not return the right amount amount");
            Assert.IsTrue(collection[0].When == first, "Did not get returned in the right order");
            Assert.IsTrue(collection[1].When == second, "Did not get returned in the right order");
        }
        [TestMethod]
        public void TestFindLatestCyclingSessionNotEnoughInDatabase()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            DateTime first = new DateTime(2000, 5, 12);
            DateTime second = new DateTime(2008, 5, 12);
            DateTime third = DateTime.Now;

            m.AddCyclingTraining(second, null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.CityBike);
            m.AddCyclingTraining(first, null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.IndoorBike);
            m.AddCyclingTraining(third, null, new TimeSpan(2, 0, 0), null, null, TrainingType.Endurance, null, BikeType.MountainBike);
            m.AddRunningTraining(DateTime.Now, 2000, new TimeSpan(1, 0, 0), null, TrainingType.Endurance, null);

            //Act
            var collection = m.GetPreviousCyclingSessions(5);

            //Assert
            Assert.IsTrue(collection.Count == 3, "Did not return the right amount amount");
            Assert.IsTrue(collection[0].When == first, "Did not get returned in the right order");
            Assert.IsTrue(collection[1].When == second, "Did not get returned in the right order");
        }

        [TestMethod]
        public void TestFindLatestRunningSessionsNotEnoughInDatabase()
        {
            //Arrange
            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContextTest()));
            DateTime first = new DateTime(2000, 5, 12);
            DateTime second = new DateTime(2008, 5, 12);
            DateTime third = DateTime.Now;

            m.AddRunningTraining(second, 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, null);
            m.AddRunningTraining(first, 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, null);
            m.AddRunningTraining(third, 10000, new TimeSpan(2, 0, 0), null, TrainingType.Endurance, null);
            m.AddCyclingTraining(DateTime.Now, 20, new TimeSpan(1, 0, 0), null, null, TrainingType.Endurance, null, BikeType.MountainBike);

            //Act
            var collection = m.GetPreviousRunningSessions(5);

            //Assert
            Assert.IsTrue(collection.Count == 3, "Did not return the right amount amount");
            Assert.IsTrue(collection[0].When == first, "Did not get returned in the right order");
            Assert.IsTrue(collection[1].When == second, "Did not get returned in the right order");
        }
    }
}
