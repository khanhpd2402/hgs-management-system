import { FallbackErrorBoundary } from "@/components/FallbackErrorBoundary";
import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import ScheduleTable from "@/pages/Schedule/Schedule";
import SendMessagePHHS from "@/pages/SendMessage/PHHS/SendMessagePHHS";
import SendMessageTeacher from "@/pages/SendMessage/Teacher/SendMessageTeacher";
import AttendanceTable from "@/pages/Teacher/Attendance/AttendanceTable";
import MarkReportTable from "@/pages/Teacher/MarkReport/MarkReportTable";
import AddTeacher from "@/pages/Teacher/Profile/AddTeacher";
import { lazy, Suspense } from "react";
import { ErrorBoundary } from "react-error-boundary";
import { createBrowserRouter, RouterProvider } from "react-router";

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

const AppRouter = () => {
  const routes = privateRouter;

  const router = createBrowserRouter([...routes]);

  return <RouterProvider router={router} />;
};

const privateRouter = [
  {
    element: <DefaultLayout />,

    children: [
      {
        path: "/",
        element: <div>Home</div>,
      },
      {
        path: "/teacher/profile",
        element: (
          <ErrorBoundary fallback={<FallbackErrorBoundary />}>
            <Suspense fallback={<div>Loading...</div>}>
              <TeacherTable />
            </Suspense>
          </ErrorBoundary>
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
          <Suspense fallback={<div>Loading...</div>}>
            <MarkReportTable />
          </Suspense>
        ),
      },
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
        path: "/student/score",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <StudentScore />
          </Suspense>
        ),
      },
      {
        path: "system/schedule",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <ScheduleTable></ScheduleTable>
          </Suspense>

        ),
      },
      {
        path: "system/schedule",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <ScheduleTable></ScheduleTable>
          </Suspense>

        ),
      },
      {
        path: "contact/SendToParent",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <SendMessagePHHS></SendMessagePHHS>
          </Suspense>

        ),
      },
      {
        path: "contact/SendToTeacher",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <SendMessageTeacher></SendMessageTeacher>
          </Suspense>

        ),
      },

    ],
  },
];

export default AppRouter;
