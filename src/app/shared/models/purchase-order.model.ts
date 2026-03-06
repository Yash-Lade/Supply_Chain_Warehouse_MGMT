export interface PurchaseOrder {
  id: number;
  poNumber: string;
  vendorName: string;
  totalAmount: number;
  status: 'Draft' | 'PendingApproval' | 'Approved' | 'Received' | 'Rejected';
  createdDate: string;

  level1Approved?: boolean;
  level2Approved?: boolean;
  level3Approved?: boolean;

}