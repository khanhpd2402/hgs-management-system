import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import AddGradeBatch from "./AddGradeBatch";
import GradeBatchDetail from "./GradeBatchDetail";

export default function GradeBatch() {
  const [gradeBatches, setGradeBatches] = useState([
    {
      id: 1,
      name: "Đợt nhập điểm học kỳ 1 năm 2023-2024",
      startDate: new Date("2023-09-01"),
      endDate: new Date("2023-12-31"),
      status: "active",
      scoreTypes: {
        frequent: { enabled: true, count: "3" },
        midterm: { enabled: true },
        final: { enabled: true },
      },
    },
    {
      id: 2,
      name: "Đợt nhập điểm học kỳ 2 năm 2022-2023",
      startDate: new Date("2023-01-01"),
      endDate: new Date("2023-05-31"),
      status: "completed",
      scoreTypes: {
        frequent: { enabled: true, count: "4" },
        midterm: { enabled: true },
        final: { enabled: true },
      },
    },
  ]);

  const [selectedBatch, setSelectedBatch] = useState(null);
  const [detailModalOpen, setDetailModalOpen] = useState(false);

  // Check and update batch statuses based on current date
  useEffect(() => {
    const now = new Date();
    const updatedBatches = gradeBatches.map((batch) => {
      const endDate = new Date(batch.endDate);
      if (now > endDate && batch.status === "active") {
        return { ...batch, status: "completed" };
      }
      return batch;
    });

    // Only update state if there were changes
    if (JSON.stringify(updatedBatches) !== JSON.stringify(gradeBatches)) {
      setGradeBatches(updatedBatches);
    }
  }, [gradeBatches]);

  const handleAddGradeBatch = (newBatch) => {
    // Check if the new batch should be marked as completed based on end date
    const now = new Date();
    const status = now > new Date(newBatch.endDate) ? "completed" : "active";

    setGradeBatches([
      ...gradeBatches,
      {
        id: gradeBatches.length + 1,
        ...newBatch,
        status,
      },
    ]);
  };

  const handleViewDetails = (batch) => {
    setSelectedBatch(batch);
    setDetailModalOpen(true);
  };

  const handleSaveBatchEdit = (editedBatch) => {
    // Check if status needs to be updated based on end date
    const now = new Date();
    const endDate = new Date(editedBatch.endDate);

    // Update status based on end date
    let updatedBatch = { ...editedBatch };
    if (now > endDate && updatedBatch.status === "active") {
      updatedBatch.status = "completed";
    } else if (now <= endDate && updatedBatch.status === "completed") {
      updatedBatch.status = "active";
    }

    setGradeBatches(
      gradeBatches.map((batch) =>
        batch.id === updatedBatch.id ? updatedBatch : batch,
      ),
    );
    setSelectedBatch(updatedBatch);
  };

  const formatDate = (date) => {
    return new Date(date).toLocaleDateString("vi-VN");
  };

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

  return (
    <div className="container mx-auto py-6">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-bold">Quản lý đợt nhập điểm</h1>
        <AddGradeBatch onAddBatch={handleAddGradeBatch} />
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        {gradeBatches.map((batch) => (
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
        ))}
      </div>

      {/* Detail Modal Component */}
      {selectedBatch && (
        <GradeBatchDetail
          batch={selectedBatch}
          open={detailModalOpen}
          onOpenChange={setDetailModalOpen}
          onSave={handleSaveBatchEdit}
        />
      )}
    </div>
  );
}
