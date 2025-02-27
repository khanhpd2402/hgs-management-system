import { useState } from "react";
import { Link } from "react-router-dom";
import { Menu, ChevronDown, ChevronRight, Home, Users, Settings, ComputerIcon } from "lucide-react";

const menuItems = [
  { label: "Trang chủ", icon: Home, path: "/" },
  { label: "Quản trị", icon: Settings, path: "/admin" },
  {
    label: "Hệ thống",
    icon: ComputerIcon,
    children: [
      { label: "Khai báo dữ liệu", path: "/system/data" },
      { label: "Khai báo mẫu nhận xét", path: "/system/review" },
      { label: "Thời khóa biểu", path: "/system/schedule" },
    ],
  },
  {
    label: "Cán bộ",
    icon: Users,
    children: [
      { label: "Hồ sơ cán bộ", path: "/staff/profile" },
      { label: "Phân công giảng dạy", path: "/staff/teaching" },
      { label: "Phân công giáo vụ", path: "/staff/academic" },
      { label: "Công việc kiêm nhiệm", path: "/staff/tasks" },
      { label: "Phân công chủ nhiệm", path: "/staff/homeroom" },
      { label: "Khen thưởng - Kỷ luật", path: "/staff/awards" },
    ],
  },
];

export default function Sidebar({ isOpen, setIsOpen }) {
  const [openMenus, setOpenMenus] = useState({});

  const toggleMenu = (label) => {
    if (isOpen) {
      setOpenMenus((prev) => ({ ...prev, [label]: !prev[label] }));
    }
  };

  return (
    <div className={`fixed top-0 left-0 h-full bg-teal-700 text-white transition-all duration-300 ${isOpen ? "w-64" : "w-16"}`}>
      {/* Toggle Sidebar Button */}
      <button className="p-4 text-white focus:outline-none" onClick={() => setIsOpen(!isOpen)}>
        <Menu className="h-6 w-6" />
      </button>

      {/* Sidebar Menu */}
      <nav className="space-y-2 p-2">
        {menuItems.map((item) => (
          <div key={item.label}>
            {item.path ? (
              // Nếu là menu chính có link, dùng Link
              <Link
                to={item.path}
                className="flex items-center space-x-3 w-full rounded-md p-2 hover:bg-teal-600 transition-all"
              >
                <item.icon className="h-5 w-5" />
                <span className={`truncate transition-all duration-200 ${isOpen ? "opacity-100" : "opacity-0 w-0 hidden"}`}>
                  {item.label}
                </span>
              </Link>
            ) : (
              // Nếu có submenu, dùng button để toggle
              <button
                className="flex items-center space-x-3 w-full rounded-md p-2 hover:bg-teal-600 transition-all"
                onClick={() => toggleMenu(item.label)}
              >
                <item.icon className="h-5 w-5" />
                <span className={`truncate transition-all duration-200 ${isOpen ? "opacity-100" : "opacity-0 w-0 hidden"}`}>
                  {item.label}
                </span>
                {item.children && isOpen && (
                  <div>{openMenus[item.label] ? <ChevronDown className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}</div>
                )}
              </button>
            )}

            {/* Submenu */}
            {item.children && (
              <div className={`overflow-hidden transition-all ${openMenus[item.label] && isOpen ? "mt-2 ml-6 opacity-100" : "h-0 opacity-0 hidden"}`}>
                {item.children.map((child) => (
                  <Link
                    key={child.label}
                    to={child.path}
                    className="block w-full rounded-md p-2 text-left hover:bg-teal-500"
                  >
                    {child.label}
                  </Link>
                ))}
              </div>
            )}
          </div>
        ))}
      </nav>
    </div>
  );
}
