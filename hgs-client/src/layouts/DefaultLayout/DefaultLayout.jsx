import "./DefaultLayout.scss";
import Sidebar from "@/components/sidebar/Sidebar";
import { useState } from "react";
import { Outlet } from "react-router";

const DefaultLayout = () => {
  const [sidebarOpen, setSidebarOpen] = useState(true);

  return (
    <div className="flex h-screen">
      <Sidebar isOpen={sidebarOpen} setIsOpen={setSidebarOpen} />
      <div className={`flex-1 ${sidebarOpen ? "ml-64" : "ml-16"} `}>
        <div className="h-full">
          <div className="container mx-auto px-4 py-4">
            <Outlet />
          </div>
        </div>
      </div>
    </div>
  );
};

export default DefaultLayout;
