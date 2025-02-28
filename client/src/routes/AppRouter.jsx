import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import { lazy, Suspense } from "react";
import { createBrowserRouter, RouterProvider } from "react-router";

const EmployeeTable = lazy(
  () => import("@/pages/Employee/EmployeeProfile/EmployeeTable"),
);
const StudentTable = lazy(
  () => import("@/pages/Student/StudentProfile/StudentTable"),
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
        path: "/employee",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <EmployeeTable />
          </Suspense>
        ),
      },
      {
        path: "/student",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <StudentTable />
          </Suspense>
        ),
      },
    ],
  },
];

export default AppRouter;
