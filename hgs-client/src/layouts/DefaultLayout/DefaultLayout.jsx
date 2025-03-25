import Header from "@/components/header/Header";
import "./DefaultLayout.scss";
import Sidebar from "@/components/sidebar/Sidebar";
import { useState } from "react";
import { Outlet, useOutletContext } from "react-router";

const DefaultLayout = () => {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const [currentYear, setCurrentYear] = useState(null);

  return (
    <div className="flex h-screen overflow-x-clip">
      <Sidebar isOpen={sidebarOpen} setIsOpen={setSidebarOpen} />
      <div
        className={`flex-1 ${sidebarOpen ? "ml-64" : "ml-16"} relative overflow-hidden`}
      >
        <div className="h-full overflow-x-clip overflow-y-scroll">
          <div className="container mx-auto px-4 py-4">
            <Header setCurrentYear={setCurrentYear} />
            <Outlet context={{ currentYear }} />
          </div>
        </div>
      </div>
    </div>
  );
};
export function useLayout() {
  return useOutletContext();
}

export default DefaultLayout;
