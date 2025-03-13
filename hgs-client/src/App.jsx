import StudentProfile from "./pages/Student/Profile/StudentProfile";
import AddStudentForm from "./pages/Student/Profile/AddStudentForm";
import AppRouter from "./routes/AppRouter";
import StudentListScore from "./pages/Student/SummaryScore/StudentListScore";
import MarkReportTable from "./pages/Teacher/MarkReport/MarkReportTable";
import StudentScore from "./pages/Student/SummaryScore/StudentScore";

const App = () => {
  return (
    <>
      <AppRouter />
    </>
    // <AddStudentForm />
    // // <StudentProfile />
    // <MarkReportTable />
    // <StudentListScore />
    // <StudentScore />
  );
};

export default App;
