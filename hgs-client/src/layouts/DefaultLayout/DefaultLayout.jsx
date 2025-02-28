import "./DefaultLayout.scss";
import Header from "./Header";
import Footer from "./Footer";
import { Outlet } from "react-router-dom";

const DefaultLayout = () => {
  return (
    <div>
      <Header />

      <Outlet />
      <Footer />
    </div>
  );
};

export default DefaultLayout;
