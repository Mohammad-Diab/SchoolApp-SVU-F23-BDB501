using OracleConnection = Microsoft.Data.SqlClient.SqlConnection;
using OracleCommand = Microsoft.Data.SqlClient.SqlCommand;
using OracleDataReader = Microsoft.Data.SqlClient.SqlDataReader;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchoolApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string oracleConnectionString = "User Id=sa;Password=sa;Data Source=BDB501_F23";
        string connectionString = "Server=127.0.0.1;Database=BDB501_F23;User Id=sa;Password=123456;TrustServerCertificate=True";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public DataTable SqlQuery(string sql)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return dataTable;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                var grid = selectedTab.Header switch
                {
                    "Staff" => staffGrid,
                    "Staff Role" => staffRolesGrid,
                    "Study Years" => studyYearsGrid,
                    "Students" => studentsGrid,
                    "Classrooms" => classroomsGrid,
                    "Courses" => coursesGrid,
                    "Timetable" => timetableGrid,
                    "ReportCards" => reportCardsGrid,
                    "Assessments" => assessmentsGrid,
                    _ => null
                };
                if (grid != null)
                {
                    LoadDataForGrid(grid, selectedTab.Tag.ToString());
                }
            }
        }

        private void LoadDataForGrid(DataGrid dataGrid, string? tableName, string? sql = null)
        {
            if(tableName != null || sql != null)
            {
                string sqlQuery = sql != null ? sql : $"SELECT * FROM {tableName}";
                DataTable dataTable = SqlQuery(sqlQuery);
                dataGrid.ItemsSource = dataTable.DefaultView;
              //  dataGrid.SelectAll();
            }


        }

        private void SaveChange_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        private void SaveChanges()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    dataAdapter.SelectCommand = new OracleCommand("SELECT * FROM YourTableName", conn);
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                    DataTable? changes = (staffGrid.ItemsSource as DataView)?.Table;
                    if (changes != null)
                    {
                        dataAdapter.Update(changes);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Select1_Click(object sender, RoutedEventArgs e)
        {
            string sql = $"SELECT s.first_name + ' ' + s.last_name FROM Students s\r\ninner join Assessments a on a.student_id = s.student_id \r\ninner join Courses c on c.course_id = a.course_id \r\ninner join StudyYears sy on sy.academic_year = '2023-2024'\r\nwhere (c.course_name = 'Mathematics' and a.score = 100) or (c.course_name = 'Arabic' and a.score < 50)\r\ngroup by (s.first_name + ' ' + s.last_name) having COUNT(s.student_id ) = 2";
            LoadDataForGrid(statisticGrid, null, sql);

        }

        private void Select2_Click(object sender, RoutedEventArgs e)
        {
            string sql = $"SELECT \r\n c.course_id,\r\n c.course_name,\r\n STUFF(\r\n  (SELECT ', ' + s.first_name + ' ' + s.last_name\r\nFROM Timetable t\r\nJOIN Staff s ON t.staff_id = s.staff_id\r\nWHERE t.course_id = c.course_id\r\nFOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), \r\n  1, 2, ''\r\n ) AS teacher_names\r\nFROM \r\n Courses c\r\nJOIN \r\n Timetable t ON c.course_id = t.course_id\r\nGROUP BY \r\n c.course_id, c.course_name\r\nHAVING \r\n COUNT(DISTINCT t.staff_id) > 1\r\nORDER BY \r\n c.course_name;";
            LoadDataForGrid(statisticGrid, null, sql);
        }

        private void Select3_Click(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT \r\nc.course_id,\r\nc.course_name,\r\ns.student_id,\r\nCONCAT(s.first_name, ' ', s.last_name) AS student_name,\r\ntop_students.score\r\nFROM \r\n(\r\nSELECT \r\na.course_id,\r\na.student_id,\r\na.score,\r\nROW_NUMBER() OVER (PARTITION BY a.course_id ORDER BY a.score DESC) AS rn\r\nFROM \r\nAssessments a\r\n) top_students\r\nJOIN \r\nCourses c ON top_students.course_id = c.course_id\r\nJOIN \r\nStudents s ON top_students.student_id = s.student_id\r\nWHERE \r\ntop_students.rn = 1\r\nORDER BY \r\nc.course_name;";
            LoadDataForGrid(statisticGrid, null, sql);
        }
    }
}