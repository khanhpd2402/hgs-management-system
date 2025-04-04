import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import AddGradeBatch from "./addgradebatch";
import GradeBatchDetail from "./GradeBatchDetail";
import { cn } from "@/lib/utils";
import { useSemestersByAcademicYear } from "@/services/common/queries";
import { useGradeBatchs } from "@/services/principal/queries";
import { formatDate } from "@/helpers/formatDate";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";

export default function GradeBatch() {
  const { currentYear } = useLayout();
  const [semester, setSemester] = useState(null);

  const gradeBatchs = useGradeBatchs();
  const semesterQuery = useSemestersByAcademicYear(currentYear?.academicYearID);
  const semesters = semesterQuery.data || [];
  const currentGradeBatchs = gradeBatchs?.data?.filter(
    (batch) => batch.semesterId === semester,
  );
  const getStatusBadge = (status) => {
    switch (status) {
      case true:
        return <Badge className="bg-green-500">Đang mở</Badge>;
      case false:
        return <Badge className="bg-gray-500">Đã đóng</Badge>;

      default:
        return <Badge>{status}</Badge>;
    }
  };

  useEffect(() => {
    if (semesters?.length > 0) {
      setSemester(semesters[0].semesterID);
    }
  }, [semesters, currentYear]);

  return (
    <div className="container mx-auto py-6">
      <div className="space-y-4">
        <div className="flex items-center justify-between border-b pb-4">
          <div>
            <h1 className="text-2xl font-bold">Quản lý đợt nhập điểm</h1>
            <p className="text-muted-foreground text-sm">
              Quản lý các đợt nhập điểm trong học kỳ
            </p>
          </div>
          <div>
            <AddGradeBatch semester={semester} />
          </div>
        </div>
        <div className="bg-muted text-muted-foreground inline-flex h-10 items-center justify-center rounded-lg p-1">
          {semesters?.map((sem) => (
            <button
              key={sem.semesterID}
              className={cn(
                "ring-offset-background focus-visible:ring-ring inline-flex items-center justify-center rounded-md px-8 py-1.5 text-sm font-medium whitespace-nowrap transition-all focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:outline-none disabled:pointer-events-none disabled:opacity-50",
                semester === sem.semesterID
                  ? "bg-background text-foreground shadow-sm"
                  : "hover:bg-background/50",
              )}
              onClick={() => setSemester(sem.semesterID)}
            >
              {sem.semesterName}
            </button>
          ))}
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        {currentGradeBatchs?.map((batch) => (
          <Card key={batch.id} className="overflow-hidden">
            <CardHeader className="pb-2">
              <div className="flex justify-between">
                <CardTitle className="line-clamp-1 text-lg font-semibold">
                  {batch.batchName}
                </CardTitle>
                {getStatusBadge(batch.isActive)}
              </div>
            </CardHeader>
            <Separator />
            <CardContent className="pt-4">
              <div className="mb-3 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-500">Thời gian:</span>
                  <span>
                    {formatDate(batch.startDate)} - {formatDate(batch.endDate)}
                  </span>
                </div>
              </div>

              {/* <div className="mb-3">
                <p className="mb-2 text-sm text-gray-500">Cột điểm:</p>
                <div className="flex flex-wrap gap-2">
                  {batch.scoreTypes.frequent.enabled && (
                    <Badge variant="outline">
                      Thường xuyên ({batch.scoreTypes.frequent.count})
                    </Badge>
                  )}
                  {batch.scoreTypes.midterm.enabled && (
                    <Badge variant="outline">ĐĐG GK</Badge>
                  )}
                  {batch.scoreTypes.final.enabled && (
                    <Badge variant="outline">ĐĐG CK</Badge>
                  )}
                </div>
              </div> */}

              <div className="mt-4 flex justify-end">
                <GradeBatchDetail gradeBatchId={batch.batchId} />
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Detail Modal Component */}
    </div>
  );
}
