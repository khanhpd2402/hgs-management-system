import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import AddGradeBatch from "./addgradebatch";
import GradeBatchDetail from "./GradeBatchDetail";
import { cn } from "@/lib/utils";
import { useSemestersByAcademicYear } from "@/services/common/queries";
import { useGradeBatchs } from "@/services/principal/queries";

export default function GradeBatch() {
  const gradeBatchs = useGradeBatchs();
  console.log(gradeBatchs.data);

  const academicYearId = JSON.parse(
    sessionStorage.getItem("currentAcademicYear"),
  ).academicYearID;
  const semesterQuery = useSemestersByAcademicYear(academicYearId);
  const semesters = semesterQuery.data || [];
  const [semester, setSemester] = useState(null);
  const [selectedBatch, setSelectedBatch] = useState(null);
  const [detailModalOpen, setDetailModalOpen] = useState(false);
  const gradeBatchQuery = useGradeBatchs();

  // Check and update batch statuses based on current date

  const getStatusBadge = (status) => {
    switch (status) {
      case "active":
        return <Badge className="bg-green-500">Đang diễn ra</Badge>;
      case "completed":
        return <Badge className="bg-gray-500">Đã kết thúc</Badge>;
      case "upcoming":
        return <Badge className="bg-blue-500">Sắp diễn ra</Badge>;
      default:
        return <Badge>{status}</Badge>;
    }
  };

  useEffect(() => {
    if (semesters?.length > 0 && !semester) {
      setSemester(semesters[0].semesterID);
    }
  }, [semesters, semester]);

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
        {/* {gradeBatches?.map((batch) => (
          <Card key={batch.id} className="overflow-hidden">
            <CardHeader className="pb-2">
              <div className="flex justify-between">
                <CardTitle className="line-clamp-1 text-lg font-semibold">
                  {batch.name}
                </CardTitle>
                {getStatusBadge(batch.status)}
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

              <div className="mb-3">
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
              </div>

              <div className="mt-4 flex justify-end">
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => handleViewDetails(batch)}
                >
                  Chi tiết
                </Button>
              </div>
            </CardContent>
          </Card>
        ))} */}
      </div>

      {/* Detail Modal Component */}
      {selectedBatch && (
        <GradeBatchDetail
          batch={selectedBatch}
          open={detailModalOpen}
          onOpenChange={setDetailModalOpen}
          // onSave={handleSaveBatchEdit}
        />
      )}
    </div>
  );
}
