import Login from "./pages/Login/Login";
import AppRouter from "./routes/AppRouter";
import { Toaster } from "react-hot-toast";

const App = () => {
  return (
    <div>
      <Toaster position="top-center" reverseOrder={false} />

      <AppRouter />
      {/* <Login /> */}
    </div>
  );
};

export default App;
