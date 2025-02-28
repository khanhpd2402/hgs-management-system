import "./DefaultLayout.scss";
import Sidebar from "@/components/sidebar/Sidebar";
import { useState } from "react";
import StudentTable from "@/pages/Student/StudentProfile/StudentTable";

const DefaultLayout = () => {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  return (
    <div className="flex h-screen overflow-hidden">
      <Sidebar isOpen={sidebarOpen} setIsOpen={setSidebarOpen} />
      <div
        className={`flex-1 ${sidebarOpen ? "ml-64" : "ml-16"} overflow-hidden`}
      >
        <div className="h-full overflow-auto">
          <div className="container mx-auto px-4 py-4">
            {/* <EmployeeTable /> */}
            <StudentTable />
          </div>
        </div>
      </div>
    </div>
  );
};

export default DefaultLayout;
