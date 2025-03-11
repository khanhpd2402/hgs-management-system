import { Routes, Route } from "react-router-dom";
import Sidebar from "@/components/sidebar/Sidebar";
import { useState } from "react";
import Schedule from "@/pages/Schedule/Schedule";
import ScheduleStaff from "@/pages/ScheduleStaff/ScheduleStaff";
import ScheduleStudent from "@/pages/ScheduleStudent/ScheduleStudent";

const DefaultLayout = () => {
  const [sidebarOpen, setSidebarOpen] = useState(true);

  return (
    <div className="flex">
      {/* Sidebar */}
      <Sidebar isOpen={sidebarOpen} setIsOpen={setSidebarOpen} />

      {/* Nội dung chính */}
      <div className={`flex-1 ${sidebarOpen ? "ml-64" : "ml-16"}`}>
        <div className="container mx-auto pl-8">
          <Routes>
            <Route path="/system/schedule" element={<Schedule />} />
            <Route path="/student/schedule" element={<ScheduleStudent />} />
            <Route path="/staff/schedule" element={<ScheduleStaff />} />
          </Routes>
        </div>
      </div>
    </div>
  );
};

export default DefaultLayout;
