import { FallbackErrorBoundary } from "@/components/FallbackErrorBoundary";
import AuthLayout from "@/layouts/AuthLayout/AuthLayout";
import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import Login from "@/pages/Login/Login";
import UserManagement from "@/pages/Principal/UserProfile/UserManagement";
import AttendanceTable from "@/pages/Teacher/Attendance/AttendanceTable";
import GradeBatch from "@/pages/Teacher/GradeBatch/GradeBatch";
import MarkReportTable from "@/pages/Teacher/MarkReport/MarkReportTable";
import AddTeacher from "@/pages/Teacher/Profile/AddTeacher";
import ProtectedRoute from "@/routes/ProtectedRoute";
import { lazy, Suspense } from "react";
import { ErrorBoundary } from "react-error-boundary";
import { createBrowserRouter, Navigate, RouterProvider } from "react-router";
import AuthRedirectRoute from "./AuthRedirectRoute";
import ErrorRouteComponent from "@/components/ErrorRouteComponent";
import AddStudent from "@/pages/Student/Profile/AddStudent";
import SubjectManagement from "@/pages/Principal/SubjectManagement/SubjectManagement";
import ClassManagement from "@/pages/Principal/ClassManagement/ClassManagement";
import ScheduleTeacher from "@/pages/Schedule/ScheduleTeacher/ScheduleTeacher";
import ListLeaveRequest from "@/pages/LeaveRequest/AdminLeaveRequest/ListLeaveRequest";
import TeacherLeaveRequest from "@/pages/LeaveRequest/TeacherLeaveRequest/TeacherLeaveRequest";
import LeaveRequestDetail from "@/pages/LeaveRequest/AdminLeaveRequest/LeaveRequestDetail";
import CreateTeacherLeaveRequest from "@/pages/LeaveRequest/TeacherLeaveRequest/CreateTeacherLeaveRequest";
import SubstituteTeacherAssignment from "@/pages/LeaveRequest/AdminLeaveRequest/SubstituteTeacherAssignment";
import Contact from "@/pages/contact/Contact";

const TeacherTable = lazy(() => import("@/pages/Teacher/Profile/TeacherTable"));
const StudentTable = lazy(() => import("@/pages/Student/Profile/StudentTable"));
const TeacherProfile = lazy(
  () => import("@/pages/Teacher/Profile/TeacherProfile"),
);
const StudentProfile = lazy(
  () => import("@/pages/Student/Profile/StudentProfile"),
);
const TATable = lazy(
  () => import("@/pages/Teacher/TeachingAssignment/TATable"),
);
const HTATable = lazy(
  () => import("@/pages/Teacher/HeadTeacherAssignment/HTATable"),
);

const StudentScore = lazy(
  () => import("@/pages/Student/SummaryScore/StudentScore"),
);

const ScheduleManagement = lazy(
  () => import("@/pages/Schedule/ScheduleSymtem/Schedule"),
);

const AppRouter = () => {
  const routes = [...privateRouter, ...authRoutes];
  // const routes = authRoutes;

  const router = createBrowserRouter([...routes]);

  return <RouterProvider router={router} />;
};

const authRoutes = [
  {
    element: (
      <AuthRedirectRoute>
        <AuthLayout />
      </AuthRedirectRoute>
    ),
    children: [
      {
        path: "/login",
        element: <Login />,
      },
    ],
  },
];

const adminRouter = [
  {
    path: "/system/user",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <UserManagement />
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/subject",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <SubjectManagement />
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/class",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <ClassManagement />
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/schedule",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <ScheduleManagement />
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/leave-request",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <ListLeaveRequest />
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/leave-request/:id",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <LeaveRequestDetail />
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/contact",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <Contact />
      </ProtectedRoute>
    ),
  }

];

const teacherRouter = [
  {
    path: "/teacher/profile",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <ErrorBoundary fallback={<FallbackErrorBoundary />}>
          <Suspense fallback={<div>Loading...</div>}>
            <TeacherTable />
          </Suspense>
        </ErrorBoundary>
      </ProtectedRoute>
    ),
    // errorElement: <ErrorRouteComponent />,
  },
  {
    path: "/teacher/profile/:id",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <Suspense fallback={<div>Loading...</div>}>
          <TeacherProfile />
        </Suspense>
      </ErrorBoundary>
    ),
    // errorElement: <ErrorRouteComponent />,
  },
  {
    path: "/teacher/profile/create-teacher",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <AddTeacher />
      </ErrorBoundary>
    ),
    // errorElement: <ErrorRouteComponent />,
  },
  {
    path: "/teacher/teaching-assignment",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <TATable />
      </Suspense>
    ),
  },
  {
    path: "teacher/take-attendance",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <AttendanceTable />
      </Suspense>
    ),
  },
  {
    path: "/teacher/head-teacher-assignment",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <HTATable />
      </Suspense>
    ),
  },
  {
    path: "/teacher/mark-report",
    element: (
      <ProtectedRoute requiredRoles={["Giáo viên"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <MarkReportTable />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/grade-batch",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <GradeBatch />
      </Suspense>
    ),
  },
  {
    path: "/teacher/schedule",
    element: (
      <ProtectedRoute requiredRoles={["Giáo viên"]}>
      <Suspense fallback={<div>Loading...</div>}>
        <ScheduleTeacher />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/leave-request",
    element: (
      <ProtectedRoute requiredRoles={["Giáo viên"]}>
        <TeacherLeaveRequest />
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/leave-request/create",
    element: (
      <ProtectedRoute requiredRoles={["Giáo viên"]}>
        <CreateTeacherLeaveRequest />
      </ProtectedRoute>
    ),
  },
  {

  }
];

const studentRouter = [
  {
    path: "/student/profile",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <StudentTable />
      </Suspense>
    ),
  },
  {
    path: "/student/profile/:id",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <StudentProfile />
      </Suspense>
    ),
  },
  {
    path: "/student/profile/create-student",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <AddStudent />
      </Suspense>
    ),
  },
  {
    path: "/student/score",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <StudentScore />
      </Suspense>
    ),
  },
];

const privateRouter = [
  {
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng", "Giáo viên"]}>
        <DefaultLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        path: "/home",
        element: <div>Home</div>,
      },
      {
        path: "/",
        element: <Navigate to="/home" />,
      },
      {
        path: "*",
        element: <ErrorRouteComponent />,
      },
      ...studentRouter,
      ...adminRouter,
      ...teacherRouter,
    ],
  },
];

export default AppRouter;
