import AppRouter from "./routes/AppRouter";
import { Toaster } from "react-hot-toast";

const App = () => {
  return (
    <div>
      <Toaster position="top-center" reverseOrder={false} />
      <AppRouter />
    </div>
  );
};

export default App;
