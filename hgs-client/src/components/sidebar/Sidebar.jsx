import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router";
import {
  Menu,
  ChevronDown,
  ChevronRight,
  Home,
  Users,
  Settings,
} from "lucide-react";

const menuItems = [
  {
    label: "Trang chủ",
    icon: Home,
    path: "/home",
  },
  {
    label: "Quản trị",
    icon: Settings,
    path: "/system",
    children: [
      { label: "Quản lý người dùng", path: "/system/user" },
      { label: "Quản lý lớp", path: "/system/class" },
      { label: "Quản lý môn học", path: "/system/subject" },
      { label: "Quản lý lịch giảng dạy", path: "/system/schedule" },
      { label: "Quản lý đơn xin nghỉ phép", path: "/system/leave-request" },
      { label: "Liên hệ", path: "/system/contact" },
      { label: "Quản lý năm học", path: "/system/academic-year" },
      { label: "Quản lý giáo án", path: "/system/lesson-plan" },

      {
        label: "Cấu hình môn học",
        path: "/system/teacher-subject",
      },
      // { label: "Quản lý giáo viên", path: "/admin/teacher" },
      // { label: "Quản lý học sinh", path: "/admin/student" },
    ],
  },
  {
    label: "Cán bộ",
    icon: Users,
    path: "/teacher",
    children: [
      { label: "Hồ sơ cán bộ", path: "/teacher/profile" },
      {
        label: "Phân công giảng dạy",
        path: "/teacher/teaching-assignment",
      },
      {
        label: "Phân công chủ nhiệm",
        path: "/teacher/head-teacher-assignment",
      },
      {
        label: "Điểm danh",
        path: "/teacher/take-attendance",
      },
      {
        label: "Báo cáo điểm",
        path: "/teacher/mark-report",
      },
      {
        label: "Quản lý đợt nhập điểm",
        path: "/teacher/grade-batch",
      },
      {
        label: "Lịch giảng dạy",
        path: "/teacher/schedule",
      },
      {
        label: "Quản lý đơn xin nghỉ phép",
        path: "/teacher/leave-request",
      },
      {
        label: "Lịch giảng dạy",
        path: "/teacher/lesson-plan",
      },
      {
        label: "Tạo lịch giảng dạy",
        path: "/teacher/lesson-plan/create",
      },
      {
        label: "Nộp đề thi",
        path: "/teacher/upload-exam",
      },
    ],
  },
  {
    label: "Học Sinh",
    icon: Users,
    path: "/student",

    children: [
      { label: "Hồ sơ học sinh", path: "/student/profile" },
      {
        label: "Thời khóa biểu học sinh",
        path: "/student/schedule",
      },
    ],
  },
  {
    label: "Xem điểm",
    icon: Users,
    path: "/student/score",
  },
];

export default function Sidebar({ isOpen, setIsOpen }) {
  const [openMenus, setOpenMenus] = useState({});
  const navigate = useNavigate();
  const location = useLocation();
  const currentPath = location.pathname;

  useEffect(() => {
    menuItems.forEach((item) => {
      if (
        currentPath === item.path ||
        item.children?.some((child) => child.path === currentPath)
      ) {
        setOpenMenus((prev) => ({ ...prev, [item.label]: true }));
      }
    });
  }, [currentPath]);

  const toggleMenu = (label) => {
    if (isOpen) {
      setOpenMenus((prev) => ({ ...prev, [label]: !prev[label] }));
    }
  };

  const isMenuActive = (item) => {
    return (
      currentPath === item.path ||
      item.children?.some((child) => child.path === currentPath)
    );
  };

  const isSubmenuActive = (path) => {
    return currentPath === path;
  };

  return (
    <div
      className={`fixed top-0 left-0 h-full bg-sky-800 text-white ${
        isOpen ? "w-64" : "w-16"
      }`}
    >
      {/* Button đóng/mở menu */}
      <div
        onClick={() => setIsOpen(!isOpen)}
        className={`flex h-12 cursor-pointer items-center p-2 hover:bg-sky-500 ${isOpen ? "pl-3" : "justify-center"}`}
      >
        <button className="p-2 text-white focus:outline-none">
          <Menu className="h-6 w-6" />
        </button>
      </div>

      {/* Danh sách menu */}
      <nav className="space-y-1 p-2">
        {menuItems.map((item) => (
          <div key={item.label}>
            {/* Menu chính */}
            <button
              className={`flex h-12 w-full cursor-pointer items-center justify-between rounded-md px-2 hover:bg-sky-600 ${
                isMenuActive(item) ? "bg-sky-500" : ""
              }`}
              onClick={() =>
                item.children ? toggleMenu(item.label) : navigate(item.path)
              }
            >
              <div className="flex min-w-0 items-center">
                <div className="flex w-8 shrink-0 justify-center">
                  <item.icon className="h-5 w-5" />
                </div>
                <span
                  className={`truncate ${isOpen ? "inline-block" : "hidden"}`}
                >
                  {item.label}
                </span>
              </div>
              {item.children && isOpen && (
                <div className="w-8 shrink-0">
                  {openMenus[item.label] ? (
                    <ChevronDown className="h-4 w-4" />
                  ) : (
                    <ChevronRight className="h-4 w-4" />
                  )}
                </div>
              )}
            </button>

            {/* Submenu */}
            <div
              className={`${
                openMenus[item.label] && isOpen ? "block" : "hidden"
              }`}
            >
              {item.children?.map((child) => (
                <button
                  key={child.label}
                  className={`mt-1 flex h-12 w-full cursor-pointer items-center rounded-md text-left hover:bg-sky-500 ${
                    isSubmenuActive(child.path) ? "bg-sky-500" : ""
                  }`}
                  onClick={() => navigate(child.path)}
                >
                  <div className="ml-2 w-8 shrink-0" />
                  {/* Khoảng cách để thẳng hàng với icon cha */}
                  <span className="truncate">{child.label}</span>
                </button>
              ))}
            </div>
          </div>
        ))}
      </nav>
    </div>
  );
}
