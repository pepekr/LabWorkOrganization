import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateSubgroupComponent } from './update-subgroup-component';

describe('UpdateSubgroupComponent', () => {
  let component: UpdateSubgroupComponent;
  let fixture: ComponentFixture<UpdateSubgroupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateSubgroupComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateSubgroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
