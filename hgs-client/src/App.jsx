import AppRouter from "./routes/AppRouter";
import { Toaster } from "react-hot-toast";
import AddGradeBatch from "./pages/Teacher/GradeBatch/AddGradeBatch";
import GradeBatch from "./pages/Teacher/GradeBatch/GradeBatch";
import ScheduleTable from "./pages/Schedule/Schedule";
import SendMessagePHHS from "./pages/SendMessage/PHHS/SendMessagePHHS";
import SendMessageTeacher from "./pages/SendMessage/Teacher/SendMessageTeacher";

const App = () => {
  return (
    <div>
      <Toaster position="top-center" reverseOrder={false} />
      {/* <AddGradeBatch /> */}
      {/* <SendMessageTeacher /> */}

      <AppRouter />
    </div>
  );
};

export default App;
