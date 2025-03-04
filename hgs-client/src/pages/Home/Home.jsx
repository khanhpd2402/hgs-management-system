import "./Home.scss";
import Carousel from "./Carousel";
import BestSeller from "./BestSeller";
import { client } from "../../utils/httpUtils";
import { useEffect } from "react";
import { getTodo } from "../../services/todoServer";

const Home = () => {
  useEffect(() => {
    getTodo().then(({ res, data }) => {
      console.log(res);
      console.log(data);
    });
  }, []);
  return (
    <div>
      <h1>Home</h1>
      <Carousel />
      <BestSeller />
    </div>
  );
};

export default Home;
