import { FallbackErrorBoundary } from "@/components/FallbackErrorBoundary";
import AuthLayout from "@/layouts/AuthLayout/AuthLayout";
import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import Login from "@/pages/Login/Login";

import AttendanceTable from "@/pages/Teacher/Attendance/AttendanceTable";
import GradeBatch from "@/pages/Teacher/GradeBatch/GradeBatch";
import AddTeacher from "@/pages/Teacher/Profile/AddTeacher";
import ProtectedRoute from "@/routes/ProtectedRoute";
import { lazy, Suspense } from "react";
import { ErrorBoundary } from "react-error-boundary";
import { createBrowserRouter, Navigate, RouterProvider } from "react-router";
import AuthRedirectRoute from "./AuthRedirectRoute";
import ErrorRouteComponent from "@/components/ErrorRouteComponent";
import AddStudent from "@/pages/Student/Profile/AddStudent";
import ScheduleTeacher from "@/pages/Schedule/ScheduleTeacher/ScheduleTeacher";
import ScheduleStudent from "@/pages/Schedule/ScheduleStudent/ScheduleStudent";
const AddDocument = lazy(
  () => import("@/pages/RequestLessonPlan/AddDocument/AddDocument"),
);
const TeacherLessonPlan = lazy(
  () => import("@/pages/RequestLessonPlan/Teacher/TeacherLessonPlan"),
);
// Add these lazy imports with the existing lazy imports
const ListLeaveRequest = lazy(
  () => import("@/pages/LeaveRequest/AdminLeaveRequest/ListLeaveRequest"),
);
const LeaveRequestDetail = lazy(
  () => import("@/pages/LeaveRequest/AdminLeaveRequest/LeaveRequestDetail"),
);
const Contact = lazy(() => import("@/pages/contact/Contact"));
const TeacherLeaveRequest = lazy(
  () => import("@/pages/LeaveRequest/TeacherLeaveRequest/TeacherLeaveRequest"),
);
const CreateTeacherLeaveRequest = lazy(
  () =>
    import(
      "@/pages/LeaveRequest/TeacherLeaveRequest/CreateTeacherLeaveRequest"
    ),
);
const LessonPlanList = lazy(
  () => import("@/pages/RequestLessonPlan/System/LessonPlanList"),
);
const UploadLessonPlan = lazy(
  () => import("@/pages/RequestLessonPlan/Teacher/UploadLessonPlan"),
);
const RequestLessonPlan = lazy(
  () => import("@/pages/RequestLessonPlan/System/RequestLessonPlan"),
);
const ReviewDetail = lazy(
  () => import("@/pages/RequestLessonPlan/ReviewList/ReviewDetail"),
);
const TeacherLeaveRequestDetail = lazy(
  () =>
    import(
      "@/pages/LeaveRequest/TeacherLeaveRequest/TeacherLeaveRequestDetail"
    ),
);

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

const StudentScore = lazy(
  () => import("@/pages/Student/SummaryScore/StudentScore"),
);

const ScheduleManagement = lazy(
  () => import("@/pages/Schedule/ScheduleSymtem/Schedule"),
);

const AcademicYearManagement = lazy(
  () =>
    import("@/pages/Principal/AcademicYearManagement/AcademicYearManagement"),
);

const UserManagement = lazy(
  () => import("@/pages/Principal/UserProfile/UserManagement"),
);

const SubjectManagement = lazy(
  () => import("@/pages/Principal/SubjectManagement/SubjectManagement"),
);

const ClassManagement = lazy(
  () => import("@/pages/Principal/ClassManagement/ClassManagement"),
);

const SubjectConfigForTeacher = lazy(
  () =>
    import("@/pages/Principal/SubjectConfigForTeacher/SubjectConfigForTeacher"),
);

const UploadExam = lazy(
  () => import("@/pages/Teacher/ExamProposal/UploadExam"),
);

const ExamManagement = lazy(
  () => import("@/pages/Principal/ExamManagement/ExamManagement"),
);
const TeacherListPlan = lazy(
  () => import("@/pages/RequestLessonPlan/Teacher/TeacherListPlan"),
);
const ListMarkTeacher = lazy(
  () => import("@/pages/Student/MarkReport/ListMarkTeacher"),
);

const TransferData = lazy(
  () => import("@/pages/Principal/TransferData/TransferData"),
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
      <ProtectedRoute requiredRoles={["Hiệu trưởng", "Cán bộ văn thư"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <UserManagement />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/academic-year",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng", "Hiệu phó"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <AcademicYearManagement />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/subject",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng", "Hiệu phó"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <SubjectManagement />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/exam",
    element: (
      <ProtectedRoute
        requiredRoles={["Hiệu trưởng", "Hiệu phó", "Trưởng bộ môn"]}
      >
        <Suspense fallback={<div>Loading...</div>}>
          <ExamManagement />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/class",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng", "Hiệu phó"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <ClassManagement />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/teacher-subject",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng", "Hiệu phó"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <SubjectConfigForTeacher />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/transfer-data",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng", "Hiệu phó"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <TransferData />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/schedule",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <ScheduleManagement />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/leave-request",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <ListLeaveRequest />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/leave-request/:id",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <LeaveRequestDetail />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/contact",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <Contact />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/lesson-plan",
    element: (
      <ProtectedRoute requiredRoles={["Trưởng bộ môn"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <RequestLessonPlan />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/review-detail/:planId",
    element: (
      <ProtectedRoute requiredRoles={["Hiệu trưởng"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <ReviewDetail />
        </Suspense>
      </ProtectedRoute>
    ),
  },
];

const teacherRouter = [
  {
    path: "/teacher/profile",
    element: (
      <ProtectedRoute
        requiredRoles={["Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư"]}
      >
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
        <ProtectedRoute
          requiredRoles={["Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư"]}
        >
          <Suspense fallback={<div>Loading...</div>}>
            <TeacherProfile />
          </Suspense>
        </ProtectedRoute>
      </ErrorBoundary>
    ),
    // errorElement: <ErrorRouteComponent />,
  },
  {
    path: "/teacher/profile/create-teacher",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <ProtectedRoute
          requiredRoles={["Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư"]}
        >
          <AddTeacher />
        </ProtectedRoute>
      </ErrorBoundary>
    ),
  },
  {
    path: "/system/teaching-assignment",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <ProtectedRoute requiredRoles={["Hiệu trưởng", "Hiệu phó"]}>
          <Suspense fallback={<div>Loading...</div>}>
            <TATable />
          </Suspense>
        </ProtectedRoute>
      </ErrorBoundary>
    ),
  },
  {
    path: "teacher/take-attendance",
    element: (
      <ProtectedRoute
        requiredRoles={["Hiệu phó", "Trưởng bộ môn", "Giáo viên"]}
      >
        <Suspense fallback={<div>Loading...</div>}>
          <AttendanceTable />
        </Suspense>
      </ProtectedRoute>
    ),
  },

  {
    path: "/teacher/mark-report",
    element: (
      <ProtectedRoute requiredRoles={["Trưởng bộ môn", "Giáo viên"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <ListMarkTeacher />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/system/grade-batch",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <ProtectedRoute requiredRoles={["Hiệu trưởng", "Hiệu phó"]}>
          <Suspense fallback={<div>Loading...</div>}>
            <GradeBatch />
          </Suspense>
        </ProtectedRoute>
      </ErrorBoundary>
    ),
  },
  {
    path: "/teacher/schedule",
    element: (
      <ProtectedRoute requiredRoles={["Giáo viên", "Hiệu trưởng", "Hiệu phó", "Trưởng bộ môn"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <ScheduleTeacher />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/upload-exam",
    element: (
      <ProtectedRoute requiredRoles={["Giáo viên", "Trưởng bộ môn"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <UploadExam />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/leave-request",
    element: (
      <ProtectedRoute requiredRoles={["Giáo viên", "Trưởng bộ môn"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <TeacherLeaveRequest />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/leave-request/create",
    element: (
      <ProtectedRoute requiredRoles={["Giáo viên", "Trưởng bộ môn"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <CreateTeacherLeaveRequest />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/leave-request/:id",
    element: (
      <ProtectedRoute requiredRoles={["Giáo viên", "Trưởng bộ môn"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <TeacherLeaveRequestDetail />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/lesson-plan",
    element: (
      <ProtectedRoute requiredRoles={["Trưởng bộ môn"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <TeacherLessonPlan />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/lesson-plan/create",
    element: (
      <ProtectedRoute requiredRoles={["Trưởng bộ môn", "Hiệu trưởng"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <UploadLessonPlan />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "system/lesson-plan/add-document/:planId",
    element: (
      <ProtectedRoute requiredRoles={["Trưởng bộ môn", "Hiệu trưởng"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <AddDocument />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/lesson-plan-by-teacher",
    element: (
      <ProtectedRoute requiredRoles={["Trưởng bộ môn", "Giáo viên"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <TeacherListPlan />
        </Suspense>
      </ProtectedRoute>
    ),
  },
];

const studentRouter = [
  {
    path: "/student/profile",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <ProtectedRoute
          requiredRoles={["Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư"]}
        >
          <Suspense fallback={<div>Loading...</div>}>
            <StudentTable />
          </Suspense>
        </ProtectedRoute>
      </ErrorBoundary>
    ),
  },
  {
    path: "/student/profile/:id",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <ProtectedRoute
          requiredRoles={["Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư"]}
        >
          <Suspense fallback={<div>Loading...</div>}>
            <StudentProfile />
          </Suspense>
        </ProtectedRoute>
      </ErrorBoundary>
    ),
  },
  {
    path: "/student/profile/create-student",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <ProtectedRoute
          requiredRoles={["Hiệu trưởng", "Hiệu phó", "Cán bộ văn thư"]}
        >
          <Suspense fallback={<div>Loading...</div>}>
            <AddStudent />
          </Suspense>
        </ProtectedRoute>
      </ErrorBoundary>
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
  {
    path: "/student/schedule",
    element: (
      <ProtectedRoute requiredRoles={["Phụ huynh"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <ScheduleStudent />
        </Suspense>
      </ProtectedRoute >
    ),
  },
];

const privateRouter = [
  {
    element: (
      <ProtectedRoute
        requiredRoles={["Hiệu trưởng", "Giáo viên", "Trưởng bộ môn", "Phụ huynh"]}
      >
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
