import AppRouter from "./routes/AppRouter";
import { Toaster } from "react-hot-toast";
import AddGradeBatch from "./pages/Teacher/GradeBatch/AddGradeBatch";
import GradeBatch from "./pages/Teacher/GradeBatch/GradeBatch";

const App = () => {
  return (
    <div>
      <Toaster position="top-center" reverseOrder={false} />
      {/* <AddGradeBatch /> */}
      <GradeBatch />
      {/* <AppRouter /> */}
    </div>
  );
};

export default App;
