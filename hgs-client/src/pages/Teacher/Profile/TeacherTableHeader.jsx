import ExcelImportModal from "@/components/excel/ExcelImportModal";
import ExportExcel from "@/components/excel/ExportExcel";
import PropTypes from "prop-types";
import TeacherFilter from "./TeacherFilter";
import TeacherImportExcel from "./TeacherImportExcel";

const TeacherTableHeader = ({ type = "teachers", setFilter }) => {
  return (
    <div className="mb-4 flex items-center justify-between">
      <h2 className="text-lg font-semibold">Danh sách cán bộ</h2>
      <div className="flex gap-2">
        <TeacherFilter setFilter={setFilter} />
        {/* <TeacherImportExcel type={type} /> */}
        <ExcelImportModal type={type} />
        <ExportExcel type={type} />
      </div>
    </div>
  );
};

TeacherTableHeader.propTypes = {
  type: PropTypes.string.isRequired,
  data: PropTypes.string.isRequired,
};

export default TeacherTableHeader;
