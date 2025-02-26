import EmployeeTable from "@/pages/Employee/EmployeeTable";
import "./DefaultLayout.scss";

import Sidebar from "@/components/sidebar/Sidebar";
import { useState } from "react";

const DefaultLayout = () => {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  return (
    <div className="flex">
      <Sidebar isOpen={sidebarOpen} setIsOpen={setSidebarOpen} />
      <div className={`flex-1 ${sidebarOpen ? "ml-64" : "ml-16"}`}>
        <div className="container mx-auto pl-8">
          {/* Thêm pl-8 để tạo khoảng cách bên trái */}
          <EmployeeTable />
        </div>
      </div>
    </div>
  );
};

export default DefaultLayout;
