using Microsoft.VisualStudio.TestTools.UnitTesting;
using TF_PatientTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF_PatientTest.Tests
{
    [TestClass()]
    public class PatientTests
    {
        [TestMethod()]
        public void PatientTest()
        {
            Patient p = new Patient();

            Assert.IsNotNull(p);
            Assert.IsTrue(p.Name == "", "Name: (" + p.Name + ")");
            Assert.IsTrue(System.DateTime.Compare(p.DoB, System.DateTime.Today) == 0, "Date of birth is not initialized");
            Assert.IsTrue(p.Address == "");
        }

        [TestMethod()]
        public void PatientTest1()
        {
            Patient p = new Patient(0, "Test Patient", Convert.ToDateTime("10.23.2011"), "Somewher in Finland 02000");
            Assert.IsNotNull(p);
            Assert.IsTrue(p.Name == "Test Patient");
            Assert.IsTrue(System.DateTime.Compare(p.DoB, new DateTime(2011, 10, 23)) == 0);
            Assert.IsTrue(p.Tests.Count == 0);
        }

        [TestMethod()]
        public void AddTestInfoTest()
        {
            Patient p = new Patient();

            p.AddTestInfo(new TestInfo("testtype", Convert.ToDateTime("10.23.2011"), "test_instrument", "pass"));

            Assert.IsNotNull(p);
            Assert.IsTrue(p.Tests.Count == 1);
            Assert.IsTrue(p.Tests[0].Type == "testtype");
        }
    }

    [TestClass()]
    public class PatientsDBTests
    {
        [TestMethod()]
        public void PatientsDBTest()
        {
            PatientsDB temp = new PatientsDB("tempdb_path");

            Assert.IsNotNull(temp);
            Assert.IsTrue(temp.ConnectionString == "tempdb_path");
        }

        [TestMethod()]
        public void PatientDB_CleanTest()
        {
            /*FIXME: Pass connection string for testdb */
            PatientsDB db = new PatientsDB();
            List<Patient> plist;
            db.Clear();

            plist = db.GetPatientsInfo();
            Assert.IsNotNull(plist);
            Assert.IsTrue(plist.Count == 0);
        }

        [TestMethod()]
        public void PatientsDB_AddPatientTest()
        {
            PatientsDB db = new PatientsDB();
            List<Patient> plist_read;
            db.Clear();

            Patient p_in = new Patient();
            p_in.Name = "test patient";
            p_in.DoB = new DateTime(2000, 10, 10);
            p_in.Address = "test Address";
            p_in.AddTestInfo(new TestInfo("test_type", new DateTime(1999, 10, 11), "test_instrument", "test_results"));

            db.SavePatient(p_in);

            plist_read = db.GetPatientsInfo();

            Assert.IsTrue(plist_read.Count == 1);
            Assert.IsTrue(plist_read[0].Name == p_in.Name);
            Assert.IsTrue(System.DateTime.Compare(plist_read[0].DoB, p_in.DoB) == 0);
            Assert.IsTrue(plist_read[0].Tests.Count == p_in.Tests.Count);

            /* Add New test results */
            p_in.AddTestInfo(new TestInfo("test_type2", new DateTime(1992, 10, day: 12), "new_instrument", "new_resuts"));

            db.SavePatient(p_in);

            plist_read = db.GetPatientsInfo();
            Assert.IsTrue(plist_read.Count == 1);
            Assert.IsTrue(plist_read[0].Tests.Count == 2);

            /* Add One more object */
            p_in = new Patient();
            p_in.Name = "test patient1";
            p_in.DoB = new DateTime(1900, 1, 1);
            p_in.Address = "test Address 1";
            p_in.AddTestInfo(new TestInfo("test_type1", new DateTime(1999, 10, 11), "test_instrument1", "test_results1"));

            db.SavePatient(p_in);

            plist_read = db.GetPatientsInfo();
            Assert.IsTrue(plist_read.Count == 2);
            Assert.IsTrue(plist_read[0].Tests.Count == 2);
            Assert.IsTrue(plist_read[1].Tests.Count == 1);
        }
    }
}