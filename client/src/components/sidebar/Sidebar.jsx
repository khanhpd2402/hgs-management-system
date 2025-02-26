import { useState } from "react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Sheet, SheetTrigger, SheetContent } from "@/components/ui/sheet";
import {
  ChevronDown,
  ChevronRight,
  Home,
  Users,
  Settings,
  Menu,
} from "lucide-react";

const menuItems = [
  {
    label: "Trang chủ",
    icon: Home,
  },
  {
    label: "Quản trị",
    icon: Settings,
  },
  {
    label: "Cán bộ",
    icon: Users,
    children: [
      "Hồ sơ cán bộ",
      "Phân công giảng dạy",
      "Phân công giáo vụ",
      "Công việc kiêm nhiệm",
      "Phân công chủ nhiệm",
      "Khen thưởng - Kỷ luật",
    ],
  },
];
export default function Sidebar({ isOpen, setIsOpen }) {
  const [openMenus, setOpenMenus] = useState({});

  const toggleMenu = (label) => {
    // Chỉ toggle menu khi sidebar đang mở
    if (isOpen) {
      setOpenMenus((prev) => ({ ...prev, [label]: !prev[label] }));
    }
  };

  return (
    <div
      className={`fixed top-0 left-0 h-full bg-teal-700 text-white ${
        isOpen ? "w-64" : "w-16"
      }`}
    >
      <button
        className="p-4 text-white focus:outline-none"
        onClick={() => setIsOpen(!isOpen)}
      >
        <Menu className="h-6 w-6" />
      </button>
      <nav className="space-y-2 p-2">
        {menuItems.map((item) => (
          <div key={item.label}>
            <button
              className="flex w-full items-center justify-between rounded-md p-2 hover:bg-teal-600"
              onClick={() => item.children && toggleMenu(item.label)}
            >
              <div className="flex items-center space-x-2">
                <item.icon className="h-5 w-5" />
                <span
                  className={`${isOpen ? "w-auto" : "w-0 opacity-0"} truncate`}
                >
                  {item.label}
                </span>
              </div>
              {item.children && (
                <div className={`${isOpen ? "w-auto" : "w-0 opacity-0"}`}>
                  {openMenus[item.label] ? (
                    <ChevronDown className="h-4 w-4" />
                  ) : (
                    <ChevronRight className="h-4 w-4" />
                  )}
                </div>
              )}
            </button>

            {/* Luôn render menu con nhưng ẩn bằng CSS */}
            <div
              className={`overflow-hidden ${
                openMenus[item.label] && isOpen
                  ? "submenu mt-2 ml-6"
                  : "submenu submenu-open mt-0 ml-6 opacity-0"
              }`}
            >
              {item.children?.map((child) => (
                <button
                  key={child}
                  className="block w-full rounded-md p-2 text-left hover:bg-teal-500"
                >
                  {child}
                </button>
              ))}
            </div>
          </div>
        ))}
      </nav>
    </div>
  );
}
