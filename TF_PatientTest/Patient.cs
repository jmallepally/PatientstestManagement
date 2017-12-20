using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF_PatientTest {
    public class TestInfo {
        public Int32 Id { set; get; }
        public string Type { set; get; }
        public DateTime Timestamp { set; get; }
        public string Instrument { set; get; }
        public string Result { set; get; }

        public TestInfo () {
            this.Timestamp = System.DateTime.Today;
        }
        public TestInfo (string testType, DateTime timeStamp, string instrumentUsed, string testResult) {
            this.Type = testType;
            this.Timestamp = timeStamp;
            this.Instrument = instrumentUsed;
            this.Result = testResult;
        }
    }

    public class Patient {
        public Int32 Id { set; get; }
        public string Name { set; get; }
        public DateTime DoB { set; get; }
        public string Address { set; get; }

        private List<TestInfo> _tests = new List<TestInfo> ();
        public List<TestInfo> Tests {
            get { return _tests; }
        }

        public Patient () {
            this.Name = "";
            this.Address = "";
            this.DoB = System.DateTime.Today;
        }
        public Patient (Int32 id, string name, DateTime dateOfBirth, string address) {
            this.Id = id;
            this.Name = name;
            this.DoB = dateOfBirth;
            this.Address = address;
        }
        public void AddTestInfo (TestInfo info) {
            _tests.Add (info);
        }

        public override string ToString () {
            return "Name: " + Name;
        }
    }

    public class PatientsDB {
        private string connectionString;
        public string ConnectionString { get { return connectionString; } }

        private void UpdateTestInfo (TestInfo t, Int32 PatientId, SqlConnection conn) {
            SqlCommand query = new SqlCommand ();

            if (t.Id == 0) {
                // Add new patient
                query.CommandText = "INSERT INTO TestsInfo (PatientId, TestType, Instrument, TimeStamp, Result) VALUES (@PatientID, @0, @1, @2, @3)";
                query.Parameters.Add (new SqlParameter ("PatientID", PatientId));
            } else {
                query = new SqlCommand ("UPDATE TestsInfo SET TestType = @0, Instrument=@1, TimeStamp= @2, Result = @3 WHERE ID = @4");
                query.Parameters.Add (new SqlParameter ("4", t.Id));
            }

            query.Connection = conn;

            query.Parameters.Add (new SqlParameter ("0", t.Type));
            query.Parameters.Add (new SqlParameter ("1", t.Instrument));
            query.Parameters.Add (new SqlParameter ("2", t.Timestamp));
            query.Parameters.Add (new SqlParameter ("3", t.Result));
            query.ExecuteNonQuery ();

            if (t.Id == 0) {
                /* Read ID from database */
                query.CommandText = "SELECT TOP 1 ID FROM TestsInfo ORDER BY ID DESC";
                using (SqlDataReader reader = query.ExecuteReader ()) {

                    reader.Read ();
                    t.Id = reader.GetInt32 (0);
                }
            }
        }

        public PatientsDB () {
            connectionString = Properties.Settings.Default.ConnectionString;
        }

        // For testing purpose can point to temporary database
        public PatientsDB (string connectionString) {
            this.connectionString = connectionString;
        }

        public List<Patient> GetPatientsInfo () {
            List<Patient> patients = new List<Patient> ();

            using (SqlConnection conn = new SqlConnection (connectionString)) {
                try {
                    conn.Open ();

                    SqlCommand readPatients = new SqlCommand ("SELECT ID, Name, DateOfBirth, Address FROM PatientsInfo");
                    readPatients.Connection = conn;
                    using (SqlDataReader reader = readPatients.ExecuteReader ()) {
                        while (reader.Read ()) {
                            Patient p = new Patient ();
                            p.Id = reader.GetInt32 (0);
                            p.Name = reader.GetString (1);
                            p.DoB = reader.GetDateTime (2);
                            p.Address = reader.GetString (3);

                            patients.Add (p);
                        }
                    }

                    foreach (Patient p in patients) {
                        SqlCommand readTests = new SqlCommand ("SELECT ID, TestType, Instrument, TimeStamp, Result from TestsInfo WHERE PatientID = @PatientID;");
                        readTests.Connection = conn;
                        readTests.Parameters.Add (new SqlParameter ("PatientID", Convert.ToInt32 (p.Id)));
                        using (SqlDataReader test_reader = readTests.ExecuteReader ()) {
                            while (test_reader.Read ()) {
                                TestInfo t = new TestInfo ();

                                t.Id = test_reader.GetInt32 (0);
                                t.Type = test_reader.GetString (1);
                                t.Instrument = test_reader.GetString (2);
                                t.Timestamp = test_reader.GetDateTime (3);
                                t.Result = test_reader.GetString (4);

                                p.AddTestInfo (t);
                            }
                        }
                    }
                    conn.Close ();
                } catch (SqlException ex) {
                    for (int i = 0; i < ex.Errors.Count; i++) {
                        Console.Write ("EXception: {0}: {1}", 1, ex.Errors[i].Message);
                    }
                }
            }
            return patients;
        }

        public void SavePatient (Patient p) {
            SqlCommand query = new SqlCommand ();

            if (p.Id == 0) {
                // Add new patient
                query.CommandText = "INSERT INTO PatientsInfo (Name, DateOfBirth, Address) VALUES (@0, @1, @2)";
            } else {
                query = new SqlCommand ("UPDATE PatientsInfo SET Name = @0, DateOfBirth=@1, Address= @2 WHERE ID = @3");
                query.Parameters.Add (new SqlParameter ("3", p.Id));
            }

            using (SqlConnection conn = new SqlConnection (connectionString)) {
                try {
                    conn.Open ();
                    query.Connection = conn;

                    query.Parameters.Add (new SqlParameter ("0", p.Name));
                    query.Parameters.Add (new SqlParameter ("1", p.DoB));
                    query.Parameters.Add (new SqlParameter ("2", p.Address));
                    query.ExecuteNonQuery ();

                    if (p.Id == 0) {
                        /* Read ID from database */
                        query.CommandText = "SELECT TOP 1 ID FROM PatientsInfo ORDER BY ID DESC";

                        query.Connection = conn;
                        using (SqlDataReader reader = query.ExecuteReader ()) {
                            reader.Read ();
                            p.Id = reader.GetInt32 (0);
                        }
                    }

                    foreach (TestInfo t in p.Tests) {
                        UpdateTestInfo (t, p.Id, conn);
                    }
                } catch (SqlException ex) {
                    for (int i = 0; i < ex.Errors.Count; i++) {
                        Console.Write ("EXception: {0}: {1}", 1, ex.Errors[i].Message);
                    }
                }
            }
        }

        public void DeletePatient (Patient p) {
            if (p.Id == 0)
                return;

            SqlCommand query = new SqlCommand ();

            using (SqlConnection conn = new SqlConnection (connectionString)) {

                try {
                    conn.Open ();
                    query.Connection = conn;

                    if (p.Tests.Count () > 0) {
                        foreach (TestInfo t in p.Tests) {
                            query.CommandText = "DELETE FROM TestsInfo where ID = " + Convert.ToString (t.Id);
                            query.ExecuteNonQuery ();
                        }
                    }

                    query.CommandText = "DELETE FROM PatientsInfo WHERE ID = " + Convert.ToString (p.Id);
                    query.ExecuteNonQuery ();
                } catch (SqlException ex) {
                    for (int i = 0; i < ex.Errors.Count; i++) {
                        Console.Write ("EXception: {0}: {1}", 1, ex.Errors[i].Message);
                    }
                }
            }
        }

        public void Clear () {
            SqlCommand query = new SqlCommand ();

            using (SqlConnection conn = new SqlConnection (connectionString)) {
                try {
                    conn.Open ();
                    query.Connection = conn;

                    query.CommandText = "DELETE FROM TestsInfo";
                    query.ExecuteNonQuery ();

                    query.CommandText = "DELETE FROM PatientsInfo";
                    query.ExecuteNonQuery ();

                    conn.Close ();
                } catch (SqlException ex) {
                    for (int i = 0; i < ex.Errors.Count; i++) {
                        Console.Write ("EXception: {0}: {1}", 1, ex.Errors[i].Message);
                    }
                }
            }
        }
    }
}