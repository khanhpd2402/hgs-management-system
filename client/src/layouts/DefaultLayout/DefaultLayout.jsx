import EmployeeTable from "@/pages/Employee/EmployeeTable";
import "./DefaultLayout.scss";

import Sidebar from "@/components/Sidebar";
import { useState } from "react";

const DefaultLayout = () => {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  return (
    <div className="flex">
      <Sidebar isOpen={sidebarOpen} setIsOpen={setSidebarOpen} />
      <div className={`flex-1 ${sidebarOpen ? "ml-64" : "ml-16"}`}>
        <EmployeeTable />
      </div>
    </div>
  );
};

export default DefaultLayout;
