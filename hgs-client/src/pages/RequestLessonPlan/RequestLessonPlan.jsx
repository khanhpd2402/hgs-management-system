import React, { useState } from 'react';
import UploadLessonPlan from './UploadLessonPlan.jsx';

const RequestLessonPlan = () => {
    const [activeTab, setActiveTab] = useState('upload'); // Mặc định hiển thị form upload

    const renderContent = () => {
        switch (activeTab) {
            case 'upload':
                return <UploadLessonPlan />;
            case 'list':
                return <div>Danh sách kế hoạch</div>;
            case 'pending':
                return <div>Kế hoạch chờ duyệt</div>;
            case 'approved':
                return <div>Kế hoạch đã duyệt</div>;
            case 'rejected':
                return <div>Kế hoạch bị từ chối</div>;
            default:
                return <UploadLessonPlan />;
        }
    };

    return (
        <div className="flex">
            {/* Sidebar bên trái */}
            <div className="w-64 min-h-screen bg-gray-100 p-4 border-r">
                <h2 className="text-lg font-semibold mb-4">Tác Vụ</h2>
                <div className="space-y-3">
                    <button
                        onClick={() => setActiveTab('upload')}
                        className={`block w-full py-2 px-4 rounded text-white text-center transition-colors
                            ${activeTab === 'upload' ? 'bg-blue-600' : 'bg-blue-500 hover:bg-blue-600'}`}
                    >
                        Tạo Kế Hoạch Mới
                    </button>

                    <button
                        onClick={() => setActiveTab('list')}
                        className={`block w-full py-2 px-4 rounded text-white text-center transition-colors
                            ${activeTab === 'list' ? 'bg-green-600' : 'bg-green-500 hover:bg-green-600'}`}
                    >
                        Xem Danh Sách
                    </button>

                    <button
                        onClick={() => setActiveTab('pending')}
                        className={`block w-full py-2 px-4 rounded text-white text-center transition-colors
                            ${activeTab === 'pending' ? 'bg-yellow-600' : 'bg-yellow-500 hover:bg-yellow-600'}`}
                    >
                        Kế Hoạch Chờ Duyệt
                    </button>

                    <button
                        onClick={() => setActiveTab('approved')}
                        className={`block w-full py-2 px-4 rounded text-white text-center transition-colors
                            ${activeTab === 'approved' ? 'bg-green-700' : 'bg-green-600 hover:bg-green-700'}`}
                    >
                        Kế Hoạch Đã Duyệt
                    </button>

                    <button
                        onClick={() => setActiveTab('rejected')}
                        className={`block w-full py-2 px-4 rounded text-white text-center transition-colors
                            ${activeTab === 'rejected' ? 'bg-red-600' : 'bg-red-500 hover:bg-red-600'}`}
                    >
                        Kế Hoạch Bị Từ Chối
                    </button>
                </div>
            </div>

            {/* Main content */}
            <div className="flex-1 p-6">
                {renderContent()}
            </div>
        </div>
    );
};

export default RequestLessonPlan;
