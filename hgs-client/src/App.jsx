import AppRouter from "./routes/AppRouter";
import { Toaster } from "react-hot-toast";
import AddGradeBatch from "./pages/Teacher/GradeBatch/AddGradeBatch";

const App = () => {
  return (
    <div>
      <Toaster position="top-center" reverseOrder={false} />
      <AddGradeBatch />
      {/* <AppRouter /> */}
    </div>
  );
};

export default App;
