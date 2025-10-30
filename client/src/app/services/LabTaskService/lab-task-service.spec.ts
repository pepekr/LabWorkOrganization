import { TestBed } from '@angular/core/testing';

import { LabTaskService } from './lab-task-service';

describe('LabTaskService', () => {
  let service: LabTaskService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LabTaskService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
